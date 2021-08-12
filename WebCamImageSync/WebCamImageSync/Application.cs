using Infrastructure.DirectoryManager;
using Infrastructure.EgmaCV;
using Infrastructure.FileManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebCamImageSync
{
    public class Application
    {
        private readonly IEgmaCvAdapter _egmaCvAdapter;
        private readonly IDirectoryManagerService _directoryManagerService;
        private string _folderName;

        public Application(IEgmaCvAdapter egmaCvAdapter, 
            IDirectoryManagerService directoryManagerService)
        {
            _egmaCvAdapter = egmaCvAdapter;
            _directoryManagerService = directoryManagerService;
            _folderName = AppSettingsInfo.GetCurrentValue<string>("FolderName");
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("Capturing image");
                
                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = _directoryManagerService.CreateProgramDataFilePath(_folderName, fileName);

                _egmaCvAdapter.CaptureImage(0, filePath);

                Console.WriteLine("Done");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
