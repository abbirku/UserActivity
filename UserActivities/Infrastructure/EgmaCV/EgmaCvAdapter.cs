using Emgu.CV;
using System;

namespace Infrastructure.EgmaCV
{
    public interface IEgmaCvAdapter
    {
        void CaptureImage(int camIndex, string filePath);
    }

    /// <summary>
    /// Follow:
    /// https://blog.dotnetframework.org/2020/12/29/capture-a-webcam-image-using-net-core-and-opencv/
    /// https://blog.dotnetframework.org/2020/12/30/record-mp4-h264-video-from-a-webcam-in-c-net-core/
    /// </summary>
    public class EgmaCvAdapter : IEgmaCvAdapter
    {
        public void CaptureImage(int camIndex, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new Exception("Provide a valid path of file");

            using var capture = new VideoCapture(camIndex, VideoCapture.API.DShow);
            var image = capture.QueryFrame(); //take a picture
            image.Save(filePath);
        }
    }
}
