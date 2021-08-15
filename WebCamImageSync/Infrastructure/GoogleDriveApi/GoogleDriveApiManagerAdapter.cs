using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Infrastructure.FileManager;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.GoogleDriveApi
{
    public interface IGoogleDriveApiManagerAdapter
    {
        Task<string> UploadFileAsync(string filePath, Action<IUploadProgress> uploadProgress = null);
    }

    /// <summary>
    /// Sources:
    /// https://www.daimto.com/upload-file-to-google-drive/
    /// https://www.daimto.com/google-drive-authentication-c/
    /// https://dotnetcoretutorials.com/2018/08/14/getting-a-mime-type-from-a-file-name-in-net-core/
    /// </summary>
    public class GoogleDriveApiManagerAdapter : IGoogleDriveApiManagerAdapter
    {
        private readonly string _authfilePath;
        private readonly string _directoryId;
        private readonly IFileAdapter _fileAdapter;

        public GoogleDriveApiManagerAdapter(string authfilePath, string directoryId, IFileAdapter fileAdapter)
        {
            _authfilePath = authfilePath;
            _directoryId = directoryId;
            _fileAdapter = fileAdapter;
        }

        public async Task<string> UploadFileAsync(string filePath, Action<IUploadProgress> uploadProgress = null)
        {
            var credential = GoogleCredential.FromFile(_authfilePath)
                                    .CreateScoped(DriveService.ScopeConstants.Drive);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            if (!string.IsNullOrWhiteSpace(filePath) && _fileAdapter.DoesExists(filePath))
            {
                // Upload file Metadata
                var fileMetadata = new File()
                {
                    Name = _fileAdapter.FileName(filePath),
                    Parents = new List<string>() { _directoryId }
                };

                // Create a new file on Google Drive
                await using var fsSource = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // Create a new file, with metadata and stream.
                var request = service.Files.Create(fileMetadata, fsSource, _fileAdapter.GetMimeType(filePath));
                request.Fields = "*";
                request.ChunkSize = 262144;
                if (uploadProgress != null)
                    request.ProgressChanged += uploadProgress;

                var results = await request.UploadAsync(CancellationToken.None);

                if (results.Status == UploadStatus.Failed)
                    throw new Exception($"Error uploading file: {results.Exception.Message}");

                return request.ResponseBody?.Id;
            }
            else
                throw new Exception("Provide valid file path");
        }
    }
}
