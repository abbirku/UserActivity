using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace Infrastructure.FileManager
{
    public interface IFileAdapter
    {
        string FileName(string filePath);
        bool DoesExists(string filePath);
        void CreateFile(string filePath);
        string GetMimeType(string fileName);
    }

    public class FileAdapter : IFileAdapter
    {
        public string FileName(string filePath) => Path.GetFileName(filePath);

        public void CreateFile(string filePath) => File.Create(filePath);

        public bool DoesExists(string filePath) => File.Exists(filePath);

        public string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out string contentType))
                contentType = "application/octet-stream";

            return contentType;
        }
    }
}
