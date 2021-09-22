using CoreActivities.ScreenCapture;
using CoreActivities.FileManager;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IScreenCaptureService
    {
        Task CaptureScreenAsync(int width, int height, string filePath);
    }

    public class ScreenCaptureService : IScreenCaptureService
    {
        private readonly IFileManager _fileManager;
        private readonly IScreenCapture _screenCapture;

        public ScreenCaptureService(IFileManager fileManager,
            IScreenCapture screenCapture)
        {
            _fileManager = fileManager;
            _screenCapture = screenCapture;
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
    }
}
