using CoreActivities.ScreenCapture;
using CoreActivities.FileManager;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IScreenCaptureService
    {
        void CaptureScreenCaptureActivity();
    }

    public class ScreenCaptureService : IScreenCaptureService
    {
        private readonly IFileManager _fileManager;
        private readonly IScreenCapture _screenCapture;
        private readonly IConsoleHelper _consoleHelper;

        public ScreenCaptureService(IFileManager fileManager,
            IScreenCapture screenCapture,
            IConsoleHelper consoleHelper)
        {
            _fileManager = fileManager;
            _screenCapture = screenCapture;
            _consoleHelper = consoleHelper;
        }

        public async Task CaptureScreenAsync(int width, int height, string filePath)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new Exception("Provide valid file path");

                var image = _screenCapture.CaptureUserScreen(width, height);

                if (image == null)
                    throw new Exception("Image capture not successful");

                _fileManager.SaveBitmapImage(filePath, image);
            });
        }

        public void CaptureScreenCaptureActivity()
        {
            Console.WriteLine("Provide a file name to store captured image. " +
                "Note: file name should contain (.jpg, .png etc) image extenions or you could not open the file");

            var image = _screenCapture.CaptureUserScreen(1920, 1080);

            _consoleHelper.SaveResultToFileAsync((result, filePath) =>
            {
                _fileManager.SaveBitmapImage(filePath, result);
            }, image);
        }
    }
}
