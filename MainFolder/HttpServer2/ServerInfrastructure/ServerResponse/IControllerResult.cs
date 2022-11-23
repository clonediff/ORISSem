using HttpServer2.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerResponse
{
    public interface IControllerResult
    {
        void ExecuteResult(MyContext context);
    }
}
