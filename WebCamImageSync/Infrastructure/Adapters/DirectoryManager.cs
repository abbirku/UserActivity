using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.Adapters
{
    public interface IDirectoryManager
    {
        bool Exists(string directory);
        void CreateDirectory(string directory);
    }

    public class DirectoryManager : IDirectoryManager
    {
        public void CreateDirectory(string directory) => Directory.CreateDirectory(directory);
        public bool Exists(string directory) => Directory.Exists(directory);
    }
}
