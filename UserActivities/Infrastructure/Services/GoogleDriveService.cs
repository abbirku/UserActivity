using System;
using System.Linq;
using CoreActivities.Extensions;
using System.Threading.Tasks;
using CoreActivities.DirectoryManager;
using CoreActivities.FileManager;
using CoreActivities.GoogleDriveApi;
using CoreActivities.GoogleDriveApi.Models;

namespace Infrastructure.Services
{
    public interface IGoogleDriveService
    {
        Task PrintFilesInAGoogleDirectory();
        Task UploadFileToGoogleAsync();
        Task DownloadFileAsync();
        Task DeleteFileAsync();
    }

    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly IGoogleDriveApiManager _googleDriveApiManager;
        private readonly IDirectoryService _directoryService;
        private readonly IDirectoryManager _directoryManager;
        private readonly IFile _file;
        private readonly IFileInfo _fileInfo;

        public GoogleDriveService(IGoogleDriveApiManager googleDriveApiManager,
            IDirectoryService directoryService,
            IFile file,
            IFileInfo fileInfo,
            IDirectoryManager directoryManager)
        {
            _googleDriveApiManager = googleDriveApiManager;
            _directoryManager = directoryManager;
            _directoryService = directoryService;
            _file = file;
            _fileInfo = fileInfo;
        }

        public async Task DeleteFileAsync()
        {
            var files = await _googleDriveApiManager.GetAllFilesAndFolders();
            Console.WriteLine($"To delete a file from google drive provide a number between 1 and {files.Count}");
            await PrintFilesInAGoogleDirectory();

            while (true)
            {
                Console.Write("File Number: ");
                var input = Console.ReadLine();
                var sl = int.Parse(input);

                if (0 < sl && sl <= files.Count)
                {
                    await _googleDriveApiManager.DeleteAsync(files[sl - 1].Id);
                    break;
                }
                else
                    Console.WriteLine($"Please!!! Provide a number between 1 and {files.Count}");
            }
        }

        public async Task DownloadFileAsync()
        {
            var files = await _googleDriveApiManager.GetAllFilesAndFolders();
            await PrintFilesInAGoogleDirectory();

            while (true)
            {
                Console.Write("File Number: ");
                var input = Console.ReadLine();
                var sl = int.Parse(input);

                if (0 < sl && sl <= files.Count)
                {
                    await _googleDriveApiManager.DownloadAsync(files[sl-1], $"{_directoryManager.RetrivePcDownloadFolder()}\\{files[sl - 1]}");
                    break;
                }
                else
                    Console.WriteLine($"Please!!! Provide a number between 1 and {files.Count}");
            }
        }

        public async Task PrintFilesInAGoogleDirectory()
        {
            var files = await _googleDriveApiManager.GetAllFilesAndFolders();
            var fileList = files.Select(x => x.Name).ToList();
            fileList.PrintResult();
        }

        public async Task UploadFileToGoogleAsync()
        {
            var fileList = _directoryManager.ListFilesInDirectory(_directoryManager.GetProgramDataDirectoryPath(Constants.WorkingFolder));

            Console.WriteLine($"To upload a file provide a number between 1 and {fileList.Count}");
            _directoryService.CaptureDirectoryActivity();

            while (true)
            {
                Console.Write("File Number: ");
                var input = Console.ReadLine();
                var sl = int.Parse(input);

                if (0 < sl && sl <= fileList.Count)
                {
                    await _googleDriveApiManager.UploadFileAsync(new UploadFileInfo
                    {
                        FileName = _file.FileName(fileList[sl -1]),
                        FilePath = fileList[sl - 1],
                        FileSize = _fileInfo.FileSize(fileList[sl - 1]),
                        MimeType = _file.GetMimeType(fileList[sl - 1])
                    });
                    break;
                }
                else
                    Console.WriteLine($"Please!!! Provide a number between 1 and {fileList.Count}");
            }
        }
    }
}
