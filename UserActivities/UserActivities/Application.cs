using Infrastructure.Services;
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
        private readonly IDirectoryManager _directoryManagerService;
        private readonly IGoogleDriveApiManager _googleDriveApiManagerAdapter;
        private readonly IScreenCaptureService _screenCaptureService;
        private readonly IRunningProgramService _runningProgramService;
        private readonly IActiveProgramService _activeProgramService;
        private readonly IBrowserActivityService _browserActivityService;
        private string _folderName;

        public Application(IWebCamService webCamService,
            IScreenCaptureService screenCaptureService,
            IRunningProgramService runningProgramService,
            IDirectoryManager directoryManagerService,
            IGoogleDriveApiManager googleDriveApiManagerAdapter,
            IActiveProgramService activeProgramService,
            IBrowserActivityService browserActivityService)
        {
            _webCamService = webCamService;
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
                Console.WriteLine("Starting User Activities...");
                while (true)
                {
                    Console.WriteLine("Select an active (Provide number as input)");
                    Console.WriteLine("1. Active Program");
                    Console.WriteLine("2. Browser Activity");
                    Console.WriteLine("3. Running Programs");
                    Console.WriteLine("4. Screen Capture");
                    Console.WriteLine("5. WebCam");

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
