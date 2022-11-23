using HttpServer2.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerResponse
{
    public class Ok : IControllerResult
    {
        //public 
        public Ok()
        {

        }
        public void ExecuteResult(MyContext context)
        {
            context.Context.Response.StatusCode = 200;
        }
    }
}
