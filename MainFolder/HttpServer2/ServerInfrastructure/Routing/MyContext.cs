using System.IO;
using System.Net;
using System.Reflection;

namespace HttpServer2.Routing
{
    public class MyContext
    {
        public HttpListenerContext Context;
        public ServerSettings Settings;
        public MyContext(HttpListenerContext context, ServerSettings settings)
        {
            Context = context;
            Settings = settings;
        }
    }
}
