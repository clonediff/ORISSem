using HttpServer2.Routing;
using HttpServer2.Routing.Attributes;
using HttpServer2.ServerInfrastructure.CookiesAndSessions;
using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Net;

namespace HttpServer2.Attributes
{
    public class AttributeRequestsHandler
    {
        public static readonly RouteTree routes;

        static AttributeRequestsHandler()
        {
            routes = new();

            var controllerTypes = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => Attribute.IsDefined(x, typeof(ApiControllerAttribute)));

            foreach (var controller in controllerTypes)
            {
                var controllerRoute = controller.GetCustomAttribute<ApiControllerAttribute>()!.Uri?.Trim('/') ?? controller.Name;
                foreach (var method in controller.GetMethods().Where(x => Attribute.IsDefined(x, typeof(HttpMethodAttribute))))
                {
                    var methodRoute = method.GetCustomAttribute<HttpMethodAttribute>()!.Uri?.Trim('/') ?? method.Name;
                    string fullRoute = string.Join('/', controllerRoute, methodRoute);
                    if (string.IsNullOrEmpty(methodRoute))
                        fullRoute = controllerRoute;
                    var methodArgumentToOrderIndex = GetMethodArgumentToOrderIndex(method);
                    var methodTypeAttr = method.GetCustomAttribute<HttpMethodAttribute>()!.GetType();
                    var methodType = methodTypeAttr.Name.Replace("Http", "").Replace("Attribute", "");
                    routes.AddRoute(Enum.Parse<HttpMethod>(methodType), fullRoute, method, methodArgumentToOrderIndex);
                }
            }
        }

        static Dictionary<string, (int index, Type type)> GetMethodArgumentToOrderIndex(MethodInfo method)
            => method
                .GetParameters()
                .Select((x, i) => (Name: x.Name!, i, type: x.ParameterType))
                .ToDictionary(pair => pair.Name, pair => (pair.i, pair.type));

        public async Task<bool> HandleController(MyContext context)
        {
            var request = context.Context.Request;
            var response = context.Context.Response;

            await ValidateSession(context);

            if (!routes.TryNavigate(Enum.Parse<HttpMethod>(request.HttpMethod),
                request.RawUrl!.Substring(1),
                request.InputStream, request.ContentEncoding,
                out var method, out var parameters))
                return false;

            if (!CheckCookies(context, method, out var notfound))
            {
                notfound.ExecuteResult(context);
                response.OutputStream.Close();
                return true;
            }

            AddCookieValuesToParameters(context, method, parameters);

            var controller = Activator.CreateInstance(method.DeclaringType!);

            var ret = method.Invoke(controller, parameters);
            if (ret is Task task)
            {
                if (task.GetType().IsGenericType)
                    ret = await (dynamic)task;
                else
                {
                    await task;
                    ret = null;
                }
            }

            if (ret is IControllerResult result)
                result.ExecuteResult(context);
            else
                new DefaultJsonResult(ret).ExecuteResult(context);

            response.OutputStream.Close();

            return true;
        }

        private async Task ValidateSession(MyContext context)
        {
            var request = context.Context.Request;
            var cookie = request.Cookies[nameof(SessionId)];
            if (cookie is null)
                return;
            var cookieValue = CookieValueSerializer.Deserialize<SessionId>(cookie.Value);
            if (!await SessionManager.Inst.CheckSession(cookieValue.Id))
            {
                context.Context.Request.Cookies.Add(new Cookie { Name = nameof(SessionId), Value = cookie.Value, Expires = DateTime.Now });
                context.Context.Response.Cookies.Add(new Cookie { Name = nameof(SessionId), Value = cookie.Value, Expires = DateTime.Now });
            }
        }

        private void AddCookieValuesToParameters(MyContext context, MethodInfo method, object[] parameters)
        {
            var methodParameters = method.GetParameters()
                .Select((x, i) => (x, i))
                .Where(pair => Attribute.IsDefined(pair.x, typeof(FromCookie)));
            foreach (var (parameter, index) in methodParameters)
            {
                var fromCookieAttr = parameter.GetCustomAttribute<FromCookie>()!;
                var cookieName = fromCookieAttr.Type.Name.Replace("Cookie", "");
                var propertyName = fromCookieAttr.PropertyName ?? parameter.Name;
                var cookie = context.Context.Request.Cookies[cookieName];
                if (cookie is null)
                    continue;
                var cookieValue = CookieValueSerializer.Deserialize(cookie.Value, fromCookieAttr.Type);
                var property = cookieValue.GetType().GetProperty(propertyName!) ??
                    throw new ArgumentException($"{fromCookieAttr.Type} doesn't contains property {propertyName}");
                parameters[index] = Convert.ChangeType(property.GetValue(cookieValue), parameter.ParameterType)!;
            }
        }

        bool CheckCookies(MyContext context, MethodInfo method, out IControllerResult notFound)
        {
            var request = context.Context.Request;

            var checkCookies = method
                .GetCustomAttributes<CheckCookie>();

            notFound = default!;

            foreach (var checkCookie in checkCookies)
            {
                var cookieType = checkCookie.Type;
                var cookieInst = Activator.CreateInstance(cookieType) as ICookieValue;
                var cookieName = cookieType.Name.Replace("Cookie", "");
                var foundCookie = request.Cookies[cookieName];
                if (foundCookie is null)
                {
                    notFound = cookieInst!.IfNotExists;
                    return false;
                }
                if (checkCookie.PropertyName == default)
                    continue;
                var cookieValue = CookieValueSerializer.Deserialize(foundCookie.Value, cookieType);
                var property = cookieValue!.GetType().GetProperty(checkCookie.PropertyName) ??
                    throw new ArgumentException($"{cookieType} doesn't contains property {checkCookie.PropertyName}");
                if (!checkCookie.Value!.Equals(property.GetValue(cookieValue)))
                    return false;
            }

            return true;
        }
    }
}
