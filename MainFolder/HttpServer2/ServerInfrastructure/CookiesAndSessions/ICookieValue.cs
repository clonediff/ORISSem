using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrstructure.CookiesAndSessions
{
    public interface ICookieValue
    {
        IControllerResult IfNotExists { get; }

        Cookie AsCookie(TimeSpan expires);
        Cookie AsCookie();
    }
}
