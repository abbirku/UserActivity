using Infrastructure.FileManager;
using System;
using System.Threading.Tasks;

namespace Infrastructure.ScreenCapture
{
    public interface IScreenCaptureService
    {
        Task CaptureScreenAsync(int width, int height, string filePath);
    }

    public class ScreenCaptureService : IScreenCaptureService
    {
        private readonly IFileManagerService _fileManagerService;
        private readonly IScreenCaptureAdapter _screenCaptureAdapter;

        public ScreenCaptureService(IFileManagerService fileManagerService,
            IScreenCaptureAdapter screenCaptureAdapter)
        {
            _fileManagerService = fileManagerService;
            _screenCaptureAdapter = screenCaptureAdapter;
        }

        public async Task CaptureScreenAsync(int width, int height, string filePath)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new Exception("Provide valid file path");

                var image = _screenCaptureAdapter.CaptureUserScreen(width, height);

                if (image == null)
                    throw new Exception("Image capture not successful");

                _fileManagerService.SaveBitmapImage(filePath, image);
            });
        }
    }
}
