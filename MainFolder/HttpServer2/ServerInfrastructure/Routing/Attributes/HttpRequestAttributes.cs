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
    public abstract class HttpMethodAttribute : Attribute
    {
        public string Uri { get; set; }
        public HttpMethodAttribute() { }
        public HttpMethodAttribute(string uri)
        {
            Uri = uri;
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGETAttribute : HttpMethodAttribute
    {
        public HttpGETAttribute(string uri) 
            : base(uri)
        { }

        public HttpGETAttribute() : base()
        { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPOSTAttribute : HttpMethodAttribute
    {
        public HttpPOSTAttribute(string uri)
            : base(uri)
        { }

        public HttpPOSTAttribute() : base()
        { }
    }
}
