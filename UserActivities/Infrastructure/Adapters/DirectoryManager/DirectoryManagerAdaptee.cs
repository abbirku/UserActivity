using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CoreActivities.DirectoryManager
{
    public class DirectoryManagerAdaptee
    {
        public string CommonApplicationPath => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public bool Exists(string directory) => Directory.Exists(directory);
        public void CreateDirectory(string directory) => Directory.CreateDirectory(directory);
        public IList<string> FilesInDirectory(string directoryPath, string pattern) => Directory.GetFiles(directoryPath, pattern, SearchOption.AllDirectories);
        public string RetrivePcDownloadFolder() => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Downloads";
    }
}
