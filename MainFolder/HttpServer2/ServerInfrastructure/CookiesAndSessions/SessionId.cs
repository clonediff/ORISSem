using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.ServerInfrstructure.ServerResponse;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrastructure.CookiesAndSessions
{
    internal class SessionId : ICookieValue
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public IControllerResult IfNotExists { get; } = new NotAuthorized();

        public Cookie AsCookie(TimeSpan expiresIn)
        {
            var value = CookieValueSerializer.Serialize(this);
            return new Cookie { Name = "SessionId", Value = value, Expires = DateTime.Now + expiresIn };
        }

        public Cookie AsCookie()
        {
            var value = CookieValueSerializer.Serialize(this);
            return new Cookie { Name = "SessionId", Value = value };
        }
    }
}
