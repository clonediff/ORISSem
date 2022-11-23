using System.IO;

namespace HttpServer2
{
    static class ContentTypeIdentifier
    {
        public static string GetContentType(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".html":
                    return "text/html; charset=UTF-8";
                case ".css":
                    return "text/css; charset=UTF-8";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".svg":
                    return "image/svg+xml";
                case ".js":
                    return "application/javascript";
                default:
                    return "text/plain; charset=UTF-8";
            }
        }
    }
}
