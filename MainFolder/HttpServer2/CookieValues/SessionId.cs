using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.ServerInfrstructure.ServerResponse;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.CookieValues
{
    internal class SessionId : ICookieValue
    {
        public Guid Id { get; set; }

        public IControllerResult IfNotExists { get; } = new NotAuthorized();

        public Cookie AsCookie(TimeSpan expires)
        {
            var value = CookieValueSerializer.Serialize(this);
            return new Cookie { Name = "SessionId", Value = value, Expires = DateTime.Now + expires };
        }

        public Cookie AsCookie()
        {
            var value = CookieValueSerializer.Serialize(this);
            return new Cookie { Name = "SessionId", Value = value };
        }
    }
}
