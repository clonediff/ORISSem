using System.IO;
using System.Net;
using System.Text;

namespace HttpServer2
{
    class DefaultResponseSender
    {
        public bool SendStaticFile(HttpListenerContext context, ServerSettings settings)
        {
            var request = context.Request;
            var response = context.Response;

            var filePath = GetFilePath(request.RawUrl!.Substring(1), settings);

            if (!File.Exists(filePath))
                return false;

            byte[] buffer = File.ReadAllBytes(filePath);
            response.ContentType = ContentTypeIdentifier.GetContentType(filePath);
            
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            return true;
        }

        public string GetFilePath(string rawUrl, ServerSettings settings)
        {
            var filePath = Path.Combine(settings.TemplatesFolder, rawUrl);
            return filePath;
        }
    }
}
