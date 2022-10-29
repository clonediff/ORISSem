using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiControllerAttribute : Attribute
    {
        public string Uri { get; set; }
        public ApiControllerAttribute() { }
        public ApiControllerAttribute(string uri)
        {
            Uri = uri;
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : Attribute
    {
        public string Uri { get; set; }
        public HttpGetAttribute() { }
        public HttpGetAttribute(string uri)
        {
            Uri = uri;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : Attribute
    {
        public string Uri { get; set; }
        public HttpPostAttribute() { }
        public HttpPostAttribute(string uri)
        {
            Uri = uri;
        }

    }
}
