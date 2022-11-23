using HttpServer2.Routing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace HttpServer2.ServerResponse
{
    public class DefaultJsonResult : IControllerResult
    {
        //static 
        object data;
        public DefaultJsonResult(object data) => this.data = data;
        public void ExecuteResult(MyContext context)
        {
            context.Context.Response.WriteBody(
                context.Context.Request.ContentEncoding.GetBytes(JsonSerializer.Serialize(data)), 
                "application/json");
        }
    }
}
