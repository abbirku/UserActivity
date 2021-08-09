﻿using System;

namespace Infrastructure.DirectoryManager
{
    public interface IDirectoryManagerService
    {
        bool ChecknCreateDirectory(string directoryPath);
        string GetProgramDataDirectoryPath(string appFolder);
        string CreateProgramDataFilePath(string folderName, string fileName);
    }

    public class DirectoryManagerService : IDirectoryManagerService
    {
        private readonly IDirectoryManagerAdapter _directoryManagerAdapter;

        public DirectoryManagerService(IDirectoryManagerAdapter directoryManagerAdapter)
            => _directoryManagerAdapter = directoryManagerAdapter;

        /// <summary>
        /// Given a directory path of a folder it creates the folder under the directory if not exists
        /// </summary>
        public bool ChecknCreateDirectory(string directoryPath)
        {
            if (!_directoryManagerAdapter.Exists(directoryPath))
            {
                _directoryManagerAdapter.CreateDirectory(directoryPath);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Create a file path of given folder name which is under C:\ProgramData
        /// </summary>
        public string GetProgramDataDirectoryPath(string appFolder)
        {
            if (string.IsNullOrWhiteSpace(appFolder))
                throw new Exception("App folder string is empty.");

            return $"{_directoryManagerAdapter.CommonApplicationPath}\\{appFolder}";
        }

        /// <summary>
        /// Create a file path under given folderName in C:\ProgramData
        /// </summary>
        public string CreateProgramDataFilePath(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(fileName))
                throw new Exception("Given folder or File name is empty.");

            var directory = GetProgramDataDirectoryPath(folderName);
            var isCreated = ChecknCreateDirectory(directory);

            return isCreated ? $"{directory}\\{fileName}" : string.Empty;
        }
    }
}
