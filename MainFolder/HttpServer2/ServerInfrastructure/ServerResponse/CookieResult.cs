using HttpServer2.Routing;
using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrstructure.ServerResponse
{
    public class CookieResult : IControllerResult
    {
        List<(ICookieValue cookie, TimeSpan expires)> CookiesInfo { get; }

        public CookieResult(List<(ICookieValue cookie, TimeSpan expires)> cookiesInfo)
        {
            CookiesInfo = cookiesInfo;
        }

        public void ExecuteResult(MyContext context)
        {
            var response = context.Context.Response;
            foreach (var (cookie, expires) in CookiesInfo)
                if (expires != default)
                    response.Cookies.Add(cookie.AsCookie(expires));
                else
                    response.Cookies.Add(cookie.AsCookie());
        }
    }
}
