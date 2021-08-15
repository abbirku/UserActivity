using System;
using System.IO;

namespace Infrastructure.FileManager
{
    public interface IFileStreamAdapter
    {
        void WriteFileBytes(string filePath, byte[] file);
        byte[] ReadFileAsByte(string filePath);
        string ConvertByteToBase64String(byte[] file);
    }
    public class FileStreamAdapter : IFileStreamAdapter
    {
        public void WriteFileBytes(string filePath, byte[] file)
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            fileStream.Write(file, 0, file.Length);
        }
        public byte[] ReadFileAsByte(string filePath) => File.ReadAllBytes(filePath);
        public string ConvertByteToBase64String(byte[] file) => Convert.ToBase64String(file);
    }
}
