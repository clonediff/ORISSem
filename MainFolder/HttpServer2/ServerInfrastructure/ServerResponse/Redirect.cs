using HttpServer2.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerResponse
{
    public class Redirect : IControllerResult
    {
        public string RedirectUri { get; set; }
        public Redirect(string redirectUri)
        {
            RedirectUri = redirectUri;
        }

        public void ExecuteResult(MyContext context)
        {
            context.Context.Response.RedirectLocation = RedirectUri;
            context.Context.Response.StatusCode = 302;
        }
    }
}
