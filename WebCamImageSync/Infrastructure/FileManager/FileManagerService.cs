using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Infrastructure.FileManager
{
    public interface IFileManagerService
    {
        void CreateFile(string filePath);
        byte[] ReadFileAsByte(string filePath);
        void SaveByteStream(string filePath, byte[] file);
        void SaveBitmapImage(string filePath, Bitmap bitmap);
    }

    public class FileManagerService : IFileManagerService
    {
        private readonly IFileAdapter _fileAdapter;
        private readonly IFileStreamAdapter _fileStreamAdapter;

        public FileManagerService(IFileAdapter fileAdapter, IFileStreamAdapter fileStreamAdapter)
        {
            _fileAdapter = fileAdapter;
            _fileStreamAdapter = fileStreamAdapter;
        }

        public void CreateFile(string path)
        {
            if (!_fileAdapter.DoesExists(path))
                _fileAdapter.CreateFile(path);
        }
        public byte[] ReadFileAsByte(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new Exception("Provide valid file path read the file stream.");

            return _fileStreamAdapter.ReadFileAsByte(filePath);
        }
        public void SaveByteStream(string filePath, byte[] file)
        {
            if (string.IsNullOrWhiteSpace(filePath) || file == null || file.Length == 0)
                throw new Exception("Provide valid file path and byte array to save the file stream.");

            _fileStreamAdapter.WriteFileBytes(filePath, file);
        }
        public void SaveBitmapImage(string filePath, Bitmap bitmap)
        {
            if (string.IsNullOrWhiteSpace(filePath) || bitmap == null)
                throw new Exception("Provide valid file path and bitmap to save the image.");

            bitmap.Save(filePath, ImageFormat.Jpeg);
        }

    }
}
