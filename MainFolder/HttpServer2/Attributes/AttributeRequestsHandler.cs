using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HttpServer2.Attributes
{
    public class AttributeRequestsHandler
    {
        internal bool MethodHandler(HttpListenerContext _httpContext)
        {
            var request = _httpContext.Request;
            var response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            string firstSegment = request.Url.Segments[1].Replace("/", "");

            var assembly = Assembly.GetExecutingAssembly();

            var apiControllerProperty = typeof(ApiControllerAttribute).GetProperty("Uri")!;


            var controller = assembly.GetTypes().FirstOrDefault(t => t.GetCustomAttribute(typeof(ApiControllerAttribute)) != null &&
                    ((apiControllerProperty.GetValue(t.GetCustomAttribute(typeof(ApiControllerAttribute))) == null && t.Name.ToLower() == firstSegment.ToLower()) ||
                    (((string)apiControllerProperty.GetValue(t.GetCustomAttribute(typeof(ApiControllerAttribute))))?.ToLower() != null &&
                    Regex.IsMatch(firstSegment.ToLower(), ((string)apiControllerProperty.GetValue(t.GetCustomAttribute(typeof(ApiControllerAttribute))))?.ToLower()) )));

            if (controller == null)
                return false;

            var otherSegments = string.Join("", request.Url.Segments.Skip(2));

            var httpMethodName = _httpContext.Request.HttpMethod.Substring(0, 1) +
                _httpContext.Request.HttpMethod.Substring(1).ToLower();

            var httpMethodTypeAttribute = assembly.GetTypes().FirstOrDefault(t => t.Name == $"Http{httpMethodName}Attribute")!;
            var httpRouteProperty = httpMethodTypeAttribute.GetProperty("Uri")!;

            var method = controller.GetMethods().FirstOrDefault(m => m.GetCustomAttribute(httpMethodTypeAttribute) != null &&
                        ((httpRouteProperty.GetValue(m.GetCustomAttribute(httpMethodTypeAttribute)) == null && m.Name.ToLower() == otherSegments.ToLower()) || 
                        ComparePattern(GetPropertySegments((string)httpRouteProperty.GetValue(m.GetCustomAttribute(httpMethodTypeAttribute))), 
                                                            request.Url.Segments.Skip(2).ToArray())));

            if (method == null) return false;

            List<object> strParams;
            switch (httpMethodName)
            {
                case "Get":
                    strParams = _httpContext.Request.Url
                                            .Segments
                                            .Skip(2)
                                            .Select(s => s.Replace("/", ""))
                                            .ToList<object>();
                    break;
                case "Post":
                    using (var stream = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        var query = stream.ReadToEnd();
                        strParams = query.Split('&').Select(x => x.Split('=')[1]).ToList<object>();
                    }
                    break;
                default:
                    return false;
            }
            if (method.GetParameters().Where(x => x.ParameterType == typeof(HttpListenerResponse)).Count() == 1)
                strParams.Add(response);

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;

            #region
            //if (request.Url.Segments.Length == 2)
            //{
            //    var httpMethodName = _httpContext.Request.HttpMethod.Substring(0, 1) +
            //    _httpContext.Request.HttpMethod.Substring(1).ToLower();

            //    var httpMethodTypeAttribute = assembly.GetTypes().FirstOrDefault(t => t.Name == $"Http{httpMethodName}Attribute")!;
            //    var httpRouteProperty = httpMethodTypeAttribute.GetProperty("Uri")!;

            //    var method = controller.GetMethods().FirstOrDefault(t => t.GetCustomAttribute(httpMethodTypeAttribute) != null &&
            //                    httpRouteProperty.GetValue(t.GetCustomAttribute(httpMethodTypeAttribute)) == null);

            //    if (method == null)
            //        return false;

            //    List<object> strParams;
            //    switch (httpMethodName)
            //    {
            //        case "Get":
            //            strParams = _httpContext.Request.Url
            //                                    .Segments
            //                                    .Skip(2)
            //                                    .Select(s => s.Replace("/", ""))
            //                                    .ToList<object>();
            //            break;
            //        case "Post":
            //            using (var stream = new StreamReader(request.InputStream, request.ContentEncoding))
            //            {
            //                var query = stream.ReadToEnd();
            //                strParams = query.Split('&').Select(x => x.Split('=')[1]).ToList<object>();
            //            }
            //            break;
            //        default:
            //            return false;
            //    }
            //    if (method.GetParameters().Where(x => x.ParameterType == typeof(HttpListenerResponse)).Count() == 1)
            //        strParams.Add(response);

            //    object[] queryParams = method.GetParameters()
            //                        .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
            //                        .ToArray();

            //    var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            //    response.ContentType = "Application/json";

            //    byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            //    response.ContentLength64 = buffer.Length;

            //    Stream output = response.OutputStream;
            //    output.Write(buffer, 0, buffer.Length);

            //    output.Close();
            //}
            #endregion

            return true;
        }

        private bool ComparePattern(string[] pattern, string[] uri)
        {
            if (pattern.Length != uri.Length)
                return false;
            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i].Contains('{') && pattern[i].Contains('}')) continue;
                if (pattern[i].Contains('{') || pattern[i].Contains('}')) return false;
                if (pattern[i] != uri[i]) return false;
            }
            return true;
        }

        private string[] GetPropertySegments(string? v)
        {
            if (string.IsNullOrEmpty(v) || v == "/")
                return new string[0];


            string[] segments;
            List<string> arrayBuilder = new List<string>();
            int num;
            for (int startIndex = 0; startIndex < v.Length; startIndex = num + 1)
            {
                num = v.IndexOf('/', startIndex);
                if (num == -1)
                    num = v.Length - 1;
                arrayBuilder.Add(v.Substring(startIndex, num - startIndex + 1));
            }
            segments = (arrayBuilder[0] == "/" ? arrayBuilder.Skip(1) : arrayBuilder).ToArray();
            return segments;
        }

        internal bool Handler(HttpListenerContext _httpContext)
        {
            // объект запроса
            HttpListenerRequest request = _httpContext.Request;

            // объект ответа
            HttpListenerResponse response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ApiControllerAttribute)))
                .FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var httpMethodName = _httpContext.Request.HttpMethod.Substring(0, 1) +
                _httpContext.Request.HttpMethod.Substring(1).ToLower();

            var httpMethodTypeAttribute = assembly.GetTypes().FirstOrDefault(t => t.Name == $"Http{httpMethodName}Attribute")!;
            var httpRouteProperty = httpMethodTypeAttribute.GetProperty("Uri")!;

            var method = controller.GetMethods().Where(t => t.GetCustomAttribute(httpMethodTypeAttribute) != null).FirstOrDefault();

            //if (method == null)
            //{
            //    method = controller.GetMethods().Where(t => httpRouteProperty.GetValue(t.GetCustomAttribute(httpMethodTypeAttribute)))
            //}

            if (method == null) return false;

            List<object> strParams;
            switch (httpMethodName)
            {
                case "Get":
                    strParams = _httpContext.Request.Url
                                            .Segments
                                            .Skip(2)
                                            .Select(s => s.Replace("/", ""))
                                            .ToList<object>();
                    break;
                case "Post":
                    using (var stream = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        var query = stream.ReadToEnd();
                        strParams = query.Split('&').Select(x => x.Split('=')[1]).ToList<object>();
                    }
                    break;
                default:
                    return false;
            }
            if (method.GetParameters().Where(x => x.ParameterType == typeof(HttpListenerResponse)).Count() == 1)
                strParams.Add(response);

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;
        }
    }
}
