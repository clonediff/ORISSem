using HttpServer2.Routing;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrstructure.ServerResponse
{
    internal class MultiResult : IControllerResult
    {
        IControllerResult[] Results { get; }

        public MultiResult(params IControllerResult[] result) => Results = result;

        public void ExecuteResult(MyContext context)
        {
            foreach (var result in Results)
                result.ExecuteResult(context);
        }
    }
}
