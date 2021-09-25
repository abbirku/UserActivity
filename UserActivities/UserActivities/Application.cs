﻿using Infrastructure.Services;
using Infrastructure.ActiveProgram;
using System;
using System.Threading.Tasks;
using Infrastructure.BrowserActivity;
using CoreActivities.EgmaCV;
using CoreActivities.DirectoryManager;
using CoreActivities.GoogleDriveApi;
using CoreActivities.ActiveProgram;
using System.Collections;
using System.Collections.Generic;
using CoreActivities.BrowserActivity;
using CoreActivities.RunningPrograms;
using System.Linq;
using CoreActivities.Extensions;

namespace UserActivities
{
    public class Application
    {
        private readonly IWebCamService _webCamService;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IScreenCaptureService _screenCaptureService;
        private readonly IRunningProgramService _runningProgramService;
        private readonly IActiveProgramService _activeProgramService;
        private readonly IBrowserActivityService _browserActivityService;
        private readonly IDirectoryService _directoryService;
        private string _folderName;

        public Application(IWebCamService webCamService,
            IScreenCaptureService screenCaptureService,
            IRunningProgramService runningProgramService,
            IGoogleDriveService googleDriveService,
            IActiveProgramService activeProgramService,
            IBrowserActivityService browserActivityService,
            IDirectoryService directoryService)
        {
            _webCamService = webCamService;
            _screenCaptureService = screenCaptureService;
            _runningProgramService = runningProgramService;
            _folderName = AppSettingsInfo.GetCurrentValue<string>("FolderName");
            _googleDriveService = googleDriveService;
            _activeProgramService = activeProgramService;
            _browserActivityService = browserActivityService;
            _directoryService = directoryService;
        }

        public async Task Run()
        {
            try
            {
                Console.WriteLine("Starting User Activities...");
                while (true)
                {
                    Console.WriteLine("Select an active (Provide number as input)");
                    Console.WriteLine("1. Active Program");
                    Console.WriteLine("2. Browser Activity");
                    Console.WriteLine("3. Running Programs");
                    Console.WriteLine("4. Screen Capture");
                    Console.WriteLine("5. WebCam");
                    Console.WriteLine("6. Enlist directory files");
                    Console.WriteLine("7. Delete a file");
                    Console.WriteLine("8. Upload a file on google drive");
                    Console.WriteLine("9. Download a file on google drive");
                    Console.WriteLine("10. Delete a file from google drive");

                    var option = Console.ReadLine();
                    if (option == "1")
                        await _activeProgramService.CaptureActiveProgramActivityAsync();
                    else if (option == "2")
                        await _browserActivityService.CaptureBrowserActivityAsync();
                    else if (option == "3")
                        await _runningProgramService.CaptureRunningPrgramActivityAsync();
                    else if (option == "4")
                        _screenCaptureService.CaptureScreenCaptureActivity();
                    else if (option == "5")
                        await _webCamService.CaptureWebCamActivityAsync();
                    else if (option == "6")
                        _directoryService.CaptureDirectoryActivity();
                    else if (option == "7")
                        _directoryService.DeleteFileActivity();
                    else if (option == "8")
                        await _googleDriveService.UploadFileToGoogleAsync();
                    else if (option == "9")
                        await _googleDriveService.DownloadFileAsync();
                    else if (option == "10")
                        await _googleDriveService.DeleteFileAsync();
                    else
                        Console.WriteLine("Provide a valid option number");

                    PrintHelper.ClearScreen();
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
