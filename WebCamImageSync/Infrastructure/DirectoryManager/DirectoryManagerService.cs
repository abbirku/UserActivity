using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DirectoryManager
{
    public interface IDirectoryManagerService
    {
        string GetProgramDataDirectoryPath(string appFolder);
        string ChecknCreateDirectory(string folderName);
        string CreateFilePath(string folderName, string fileName);
    }

    public class DirectoryManagerService : IDirectoryManagerService
    {
        private readonly IDirectoryManagerAdapter _directoryManagerAdapter;

        public DirectoryManagerService(IDirectoryManagerAdapter directoryManagerAdapter) 
            => _directoryManagerAdapter = directoryManagerAdapter;

        public string GetProgramDataDirectoryPath(string appFolder)
        {
            if (string.IsNullOrWhiteSpace(appFolder))
                throw new Exception("App folder string is empty.");

            return $"{_directoryManagerAdapter.CommonApplicationPath}\\{appFolder}";
        }

        public string ChecknCreateDirectory(string folderName)
        {
            var directory = GetProgramDataDirectoryPath(folderName);

            if (!_directoryManagerAdapter.Exists(directory))
                _directoryManagerAdapter.CreateDirectory(directory);

            return directory;
        }

        public string CreateFilePath(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new Exception("File name is empty.");

            var directory = ChecknCreateDirectory(folderName);
            var filePath = $"{directory}\\{fileName}";

            return filePath;
        }
    }
}
