using CoreActivities.ScreenCapture;
using Infrastructure.FileManager;
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
        private readonly IFileManagerService _fileManagerService;
        private readonly IScreenCapture _screenCapture;

        public ScreenCaptureService(IFileManagerService fileManagerService,
            IScreenCapture screenCapture)
        {
            _fileManagerService = fileManagerService;
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

                _fileManagerService.SaveBitmapImage(filePath, image);
            });
        }
    }
}
