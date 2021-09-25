using CoreActivities.EgmaCV;
using CoreActivities.FileManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IWebCamService
    {
        Task CaptureWebCamActivityAsync();
    }

    public class WebCamService : IWebCamService
    {
        private readonly IEgmaCv _egmaCv;
        private readonly IFileManager _fileManager;
        private readonly IConsoleHelper _consoleHelper;

        public WebCamService(IEgmaCv egmaCv,
            IFileManager fileManager,
            IConsoleHelper consoleHelper)
        {
            _egmaCv = egmaCv;
            _fileManager = fileManager;
            _consoleHelper = consoleHelper;
        }

        public async Task CaptureWebCamActivityAsync()
        {
            Console.WriteLine("Provide a file name to store captured image. " +
                "Note: file name should contain (.jpg, .png etc) image extenions or you could not open the file");

            await _consoleHelper.SaveResultToFileAsync(async (filePath) =>
            {
                await _egmaCv.CaptureImageAsync(0, filePath);
            });
        }
    }
}
