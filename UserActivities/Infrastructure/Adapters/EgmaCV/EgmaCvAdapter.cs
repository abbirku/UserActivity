using Emgu.CV;
using System;
using System.Threading.Tasks;

namespace CoreActivities.EgmaCV
{
    /// <summary>
    /// Source:
    /// https://blog.dotnetframework.org/2020/12/29/capture-a-webcam-image-using-net-core-and-opencv/
    /// https://blog.dotnetframework.org/2020/12/30/record-mp4-h264-video-from-a-webcam-in-c-net-core/
    /// </summary>
    public class EgmaCvAdapter : IEgmaCv
    {
        public async Task CaptureImageAsync(int camIndex, string filePath)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new Exception("Provide a valid path of file");

                using var capture = new VideoCapture(camIndex, VideoCapture.API.DShow);
                var image = capture.QueryFrame(); //take a picture
                image.Save(filePath);
            });
        }
    }
}
