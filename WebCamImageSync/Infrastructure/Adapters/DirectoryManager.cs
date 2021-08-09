using System;
using System.IO;

namespace Infrastructure.Adapters
{
    public interface IDirectoryManager
    {
        string CommonApplicationPath { get; }
        bool Exists(string directory);
        void CreateDirectory(string directory);
    }

    public class DirectoryManager : IDirectoryManager
    {
        public string CommonApplicationPath => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public bool Exists(string directory) => Directory.Exists(directory);
        public void CreateDirectory(string directory) => Directory.CreateDirectory(directory);
    }
}
