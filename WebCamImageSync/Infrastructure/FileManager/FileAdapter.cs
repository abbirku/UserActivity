using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.FileManager
{
    public interface IFileAdapter
    {
        bool DoesExists(string filePath);
        void CreateFile(string filePath);
    }

    public class FileAdapter : IFileAdapter
    {
        public void CreateFile(string filePath) => File.Create(filePath);

        public bool DoesExists(string filePath) => File.Exists(filePath);
    }
}
