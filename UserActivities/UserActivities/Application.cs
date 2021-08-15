using Infrastructure.DirectoryManager;
using Infrastructure.EgmaCV;
using Infrastructure.GoogleDriveApi;
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
        private readonly IScreenCaptureAdapter _screenCaptureAdapter;
        private readonly IScreenCaptureService _screenCaptureService;
        private string _folderName;

        public Application(IEgmaCvAdapter egmaCvAdapter, 
            IDirectoryManagerService directoryManagerService,
            IGoogleDriveApiManagerAdapter googleDriveApiManagerAdapter,
            IScreenCaptureAdapter screenCaptureAdapter,
            IScreenCaptureService screenCaptureService)
        {
            _egmaCvAdapter = egmaCvAdapter;
            _directoryManagerService = directoryManagerService;
            _folderName = AppSettingsInfo.GetCurrentValue<string>("FolderName");
            _googleDriveApiManagerAdapter = googleDriveApiManagerAdapter;
            _screenCaptureAdapter = screenCaptureAdapter;
            _screenCaptureService = screenCaptureService;
        }

        public async Task Run()
        {
            try
            {
                Console.WriteLine("Starting User Activities");
                
                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);

                //Capture webcam image and sync to google drive
                await _egmaCvAdapter.CaptureImageAsync(0, filePath);
                await _googleDriveApiManagerAdapter.UploadFileAsync(filePath);

                fileName = $"{Guid.NewGuid()}.jpg";
                filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);

                //Capture user screen and sync to google drive
                await _screenCaptureService.CaptureScreenAsync(1920, 1080, filePath);
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
