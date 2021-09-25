using CoreActivities.DirectoryManager;
using CoreActivities.Extensions;
using CoreActivities.FileManager;
using System;
using System.Linq;

namespace Infrastructure.Services
{
    public interface IDirectoryService
    {
        void CaptureDirectoryActivity();
        void DeleteFileActivity();
    }

    public class DirectoryService : IDirectoryService
    {
        private readonly IDirectoryManager _directoryManager;
        private readonly IFile _file;

        public DirectoryService(IDirectoryManager directoryManager,
            IFile file)
        {
            _directoryManager = directoryManager;
            _file = file;
        }

        public void CaptureDirectoryActivity()
        {
            var fileList = _directoryManager.ListFilesInDirectory(_directoryManager.GetProgramDataDirectoryPath(Constants.WorkingFolder));
            
            fileList = fileList.Select(x =>
            {
                var parts = x.Split("\\");
                var res = parts.Count() > 0 ? parts.ElementAt(parts.Count() - 1) : string.Empty;
                return res;
            }).ToList();

            fileList.PrintResult();
        }

        public void DeleteFileActivity()
        {
            var fileList = _directoryManager.ListFilesInDirectory(_directoryManager.GetProgramDataDirectoryPath(Constants.WorkingFolder));

            Console.WriteLine($"To delete a file provide a number between 1 and {fileList.Count}");
            CaptureDirectoryActivity();

            while (true)
            {
                Console.Write("File Number: ");
                var input = Console.ReadLine();
                var sl = int.Parse(input);

                if (0 < sl && sl <= fileList.Count)
                {
                    _file.DeleteFile(fileList[sl - 1]);
                    break;
                }
                else
                    Console.WriteLine($"Please!!! Provide a number between 1 and {fileList.Count}");
            }
        }
    }
}
