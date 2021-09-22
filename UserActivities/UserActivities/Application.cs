using Infrastructure.Services;
using Infrastructure.ActiveProgram;
using System;
using System.Threading.Tasks;
using Infrastructure.BrowserActivity;
using CoreActivities.EgmaCV;
using CoreActivities.DirectoryManager;
using CoreActivities.GoogleDriveApi;

namespace UserActivities
{
    public class Application
    {
        private readonly IEgmaCv _egmaCv;
        private readonly IDirectoryManager _directoryManagerService;
        private readonly IGoogleDriveApiManager _googleDriveApiManagerAdapter;
        private readonly IScreenCaptureService _screenCaptureService;
        private readonly IRunningProgramService _runningProgramService;
        private readonly IActiveProgramService _activeProgramService;
        private readonly IBrowserActivityService _browserActivityService;
        private string _folderName;

        public Application(IEgmaCv egmaCv,
            IScreenCaptureService screenCaptureService,
            IRunningProgramService runningProgramService,
            IDirectoryManager directoryManagerService,
            IGoogleDriveApiManager googleDriveApiManagerAdapter,
            IActiveProgramService activeProgramService,
            IBrowserActivityService browserActivityService)
        {
            _egmaCv = egmaCv;
            _screenCaptureService = screenCaptureService;
            _runningProgramService = runningProgramService;

            _directoryManagerService = directoryManagerService ?? throw new ArgumentNullException(nameof(directoryManagerService));
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

                

                Console.WriteLine("Done");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
