using HttpServer2.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2
{
    class DefaultResponseSender
    {
        public void SetDefaultResponse(HttpListenerContext context, ServerSettings settings)
        {
            var request = context.Request;
            var response = context.Response;

            var filePath = GetFilePath(request.RawUrl!.Substring(1), settings);

            byte[] buffer;
            if (File.Exists(filePath))
            {
                buffer = File.ReadAllBytes(filePath);
                response.ContentType = ContentTypeIdentifier.GetContentType(filePath);
            } else
            {
                buffer = Encoding.UTF8.GetBytes(Constants.NotFoundPage);
                response.ContentType = "text/html; charset=UTF-8";
                response.StatusCode = 404;
            }
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public string GetFilePath(string rawUrl, ServerSettings settings)
        {
            var filePath = "";
            if (rawUrl == "")
                filePath = Path.Combine(settings.TemplatesFolder, "index.html");
            else
                filePath = Path.Combine(settings.TemplatesFolder, rawUrl);
            return filePath;
        }
    }
}
