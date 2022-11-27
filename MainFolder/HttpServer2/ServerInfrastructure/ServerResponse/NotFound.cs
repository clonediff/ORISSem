using HttpServer2.Routing;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrastructure.ServerResponse
{
    public class NotFound : IControllerResult
    {
        public void ExecuteResult(MyContext context)
        {
            var response = context.Context.Response;

            byte[] buffer = Encoding.UTF8.GetBytes(Constants.NotFoundPage);
            response.ContentType = "text/html; charset=UTF-8";
            response.StatusCode = 404;

            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
