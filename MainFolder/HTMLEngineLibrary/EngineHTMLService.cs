using System.Text;

namespace HTMLEngineLibrary
{
    public class EngineHTMLService : IEngineHTMLService
    {
        string CreateFileAndReturnPath(string outputPath, string outputNameFile)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            var fullPath = Path.Combine(outputPath, outputNameFile);
            if (!File.Exists(fullPath))
                File.Create(fullPath).Close();

            return Path.Combine(outputPath, outputNameFile);
        }

        Stream GetStream(byte[] bytes)
        {
            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            return stream;
        }

        byte[] GetBytes(Stream template)
        {
            var result = new byte[template.Length];
            template.Read(result, 0, (int)template.Length);
            return result;
        }

        byte[] GetBytes(string template)
            => Encoding.Default.GetBytes(template);
        string GetString(byte[] bytes)
            => Encoding.Default.GetString(bytes);

        public void GenerateAndSaveInDirectory(string template, string outputPath, string outputNameFile, object model)
        {
            var filePath = CreateFileAndReturnPath(outputPath, outputNameFile);
            File.WriteAllText(filePath, GetHTML(template, model));
        }

        public void GenerateAndSaveInDirectory(Stream template, string outputPath, string outputNameFile, object model)
        {
            var stringTemplate = GetString(GetBytes(template));
            GenerateAndSaveInDirectory(stringTemplate, outputPath, outputNameFile, model);
        }

        public void GenerateAndSaveInDirectory(byte[] bytes, string outputPath, string outputNameFile, object model)
        {
            var stringTemplate = GetString(bytes);
            GenerateAndSaveInDirectory(stringTemplate, outputPath, outputNameFile, model);
        }

        public async Task GenerateAndSaveInDirectoryAsync(string template, string outputPath, string outputNameFile, object model)
        {
            var filePath = CreateFileAndReturnPath(outputPath, outputNameFile);
            await File.WriteAllTextAsync(filePath, GetHTML(template, model));
        }

        public async Task GenerateAndSaveInDirectoryAsync(Stream template, string outputPath, string outputNameFile, object model)
        {
            var stringTemplate = GetString(GetBytes(template));
            await GenerateAndSaveInDirectoryAsync(stringTemplate, outputPath, outputNameFile, model);
        }

        public async Task GenerateAndSaveInDirectoryAsync(byte[] bytes, string outputPath, string outputNameFile, object model)
        {
            var stringTemplate = GetString(bytes);
            await GenerateAndSaveInDirectoryAsync(stringTemplate, outputPath, outputNameFile, model);
        }

        public string GetHTML(string template, object model)
        {
            var tokens = new Lexer(template).Tokenize();
            var parserResult = new Parser(tokens, model).Parse();
            return parserResult.Eval().ToString()!;
        }

        public string GetHTML(Stream template, object model)
        {
            var stringTemplate = GetString(GetBytes(template));
            return GetHTML(stringTemplate, model);
        }

        public string GetHTML(byte[] bytes, object model)
            => GetHTML(GetString(bytes), model);

        public byte[] GetHTMLInByte(string template, object model)
            => GetBytes(GetHTML(template, model));

        public byte[] GetHTMLInByte(Stream template, object model)
            => GetBytes(GetHTML(GetString(GetBytes(template)), model));

        public byte[] GetHTMLInByte(byte[] bytes, object model)
            => GetBytes(GetHTML(GetString(bytes), model));

        public Stream GetHTMLInStream(string template, object model)
            => GetStream(GetBytes(GetHTML(template, model)));

        public Stream GetHTMLInStream(Stream template, object model)
        {
            var stringResult = GetHTML(GetString(GetBytes(template)), model);
            return GetStream(GetBytes(stringResult));
        }

        public Stream GetHTMLInStream(byte[] bytes, object model)
        {
            var stringResult = GetHTML(GetString(bytes), model);
            return GetStream(GetBytes(stringResult));
        }
    }
}
