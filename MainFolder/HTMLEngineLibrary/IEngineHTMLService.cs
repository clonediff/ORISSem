using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary
{
    public interface IEngineHTMLService
    {
        string GetHTML(string template, object model);
        string GetHTML(Stream template, object model);
        string GetHTML(byte[] bytes, object model);

        Stream GetHTMLInStream(string template, object model);
        Stream GetHTMLInStream(Stream template, object model);
        Stream GetHTMLInStream(byte[] bytes, object model);

        byte[] GetHTMLInByte(string template, object model);
        byte[] GetHTMLInByte(Stream template, object model);
        byte[] GetHTMLInByte(byte[] bytes, object model);

        void GenerateAndSaveInDirectory(string template, string outputPath, string outputNameFile, object model);
        void GenerateAndSaveInDirectory(Stream template, string outputPath, string outputNameFile, object model);
        void GenerateAndSaveInDirectory(byte[] bytes, string outputPath, string outputNameFile, object model);

        Task GenerateAndSaveInDirectoryAsync(string template, string outputPath, string outputNameFile, object model);
        Task GenerateAndSaveInDirectoryAsync(Stream template, string outputPath, string outputNameFile, object model);
        Task GenerateAndSaveInDirectoryAsync(byte[] bytes, string outputPath, string outputNameFile, object model);
    }
}
