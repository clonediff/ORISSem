using HttpServer2.Routing;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrastructure.ServerResponse
{
    public class BadRequest : IControllerResult
    {
        public void ExecuteResult(MyContext context)
        {
            context.Context.Response.StatusCode = 400;
        }
    }
}
