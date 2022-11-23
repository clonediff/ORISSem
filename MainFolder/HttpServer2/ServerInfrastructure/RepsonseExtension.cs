using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2
{
    public static class RepsonseExtension
    {
        public static void WriteBody(this HttpListenerResponse response, byte[] buffer, string contentType)
        {
            response.ContentLength64 = buffer.Length;
            response.ContentType = contentType;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
