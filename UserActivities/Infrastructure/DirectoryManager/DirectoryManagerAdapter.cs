using System;
using System.IO;

namespace Infrastructure.DirectoryManager
{
    public interface IDirectoryManagerAdapter
    {
        string CommonApplicationPath { get; }
        bool Exists(string directory);
        void CreateDirectory(string directory);
    }

    public class DirectoryManagerAdapter : IDirectoryManagerAdapter
    {
        public string CommonApplicationPath => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public bool Exists(string directory) => Directory.Exists(directory);
        public void CreateDirectory(string directory) => Directory.CreateDirectory(directory);
    }
}
