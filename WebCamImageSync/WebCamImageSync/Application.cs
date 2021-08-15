using Infrastructure.DirectoryManager;
using Infrastructure.EgmaCV;
using Infrastructure.FileManager;
using Infrastructure.GoogleDriveApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageSync
{
    public class Application
    {
        private readonly IEgmaCvAdapter _egmaCvAdapter;
        private readonly IDirectoryManagerService _directoryManagerService;
        private readonly IGoogleDriveApiManagerAdapter _googleDriveApiManagerAdapter;
        private string _folderName;

        public Application(IEgmaCvAdapter egmaCvAdapter, 
            IDirectoryManagerService directoryManagerService,
            IGoogleDriveApiManagerAdapter googleDriveApiManagerAdapter)
        {
            _egmaCvAdapter = egmaCvAdapter;
            _directoryManagerService = directoryManagerService;
            _folderName = AppSettingsInfo.GetCurrentValue<string>("FolderName");
            _googleDriveApiManagerAdapter = googleDriveApiManagerAdapter;
        }

        public async Task Run()
        {
            try
            {
                Console.WriteLine("Capturing image");
                
                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);

                _egmaCvAdapter.CaptureImage(0, filePath);
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
