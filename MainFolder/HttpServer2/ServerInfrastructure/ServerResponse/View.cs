using HTMLEngineLibrary;
using HttpServer2.Routing;
using System.IO;

namespace HttpServer2.ServerResponse
{
    public class View : IControllerResult
    {
        const string DefaultExtension = ".template";

        string _viewPath;
        object _model;

        public View(string viewPath) => _viewPath = viewPath;
        public View(string viewPath, object model) : this(viewPath)
            => _model = model;

        public async void ExecuteResult(MyContext context)
        {
            var fileName = _viewPath;
            if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                fileName += DefaultExtension;
            var filePath = Path.Combine(context.Settings.TemplatesFolder, fileName);
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                var result = new EngineHTMLService().GetHTMLInByte(fs, _model);
                context.Context.Response.WriteBody(
                    result,
                    "text/html");
            }
        }
    }
}
