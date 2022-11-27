using HttpServer2.Attributes;
using HttpServer2.DAOs;
using HttpServer2.Dto;
using HttpServer2.Models;
using HttpServer2.ServerInfrastructure.CookiesAndSessions;
using HttpServer2.ServerInfrastructure.ServerResponse;
using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.ServerInfrstructure.ServerResponse;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Controllers
{
    [ApiController("/profile")]
    public class ProfileController
    {
        [HttpGET("/")]
        public async Task<IControllerResult> ProfileViewerAsync(
            [FromCookie(typeof(SessionId), nameof(SessionId.Id))] Guid sessionId)
        {
            Account? acc = default;
            if (sessionId != default)
            {
                var session = await SessionManager.Inst.GetSessionInfo(sessionId);
                if (session is not null)
                    acc = await AccountsDAO.GetAccountById(session.AccountId);
            }
            if (acc is null)
                return new Redirect("/login");
            return new View("profile", acc);
        }
    }
}
