using HttpServer2.Attributes;
using HttpServer2.DAOs;
using HttpServer2.ServerInfrastructure.CookiesAndSessions;
using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Controllers
{
    [ApiController("header")]
    public class HeaderLoader
    {
        [HttpGET("/")]
        public async Task<IControllerResult> GetHeaderAsync(
            [FromCookie(typeof(SessionId), nameof(SessionId.Id))] Guid sessionId)
        {
            var model = "";
            if (sessionId != default)
            {
                var session = await SessionManager.Inst.GetSessionInfo(sessionId);
                if (session is not null)
                    model = (await AccountsDAO.GetAccountById(session.AccountId))?
                        .Login ?? "";
            }
            return new View("header", model);
        }
    }
}
