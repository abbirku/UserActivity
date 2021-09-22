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
        Task<GoogleDriveFiles> GetFilesAndFolders(string nextPageToken = null, FilesListOptionalParms optional = null);
        Task<string> UploadFileAsync(string filePath, Action<IUploadProgress> uploadProgress = null);
        Task DeleteAsync(string fileId, FilesDeleteOptionalParms optional = null);
    }

    /// <summary>
    /// Source:
    /// 1. https://github.com/LindaLawton/Google-Dotnet-Samples/blob/master/Samples/Drive%20API/v3/FilesSample.cs
    /// 2. https://developers.google.com/drive/api/v3/reference/files/list?apix_params=%7B%22fields%22%3A%22*%22%7D
    /// 3. https://www.daimto.com/google-drive-authentication-c/
    /// 4. https://developers.google.com/drive/api/v2/search-files
    /// 5. https://support.intuiface.com/hc/en-us/articles/360010850720-List-and-display-items-stored-in-a-Google-Drive-folder-using-the-Google-Drive-API
    /// </summary>

    public class GoogleDriveApiManagerAdapter : IGoogleDriveApiManagerAdapter
    {
        private readonly string _authfilePath;
        private readonly string _directoryId;
        private readonly IFileAdapter _fileAdapter;
        private readonly IFileInfoAdapter _fileInfoAdapter;
        private readonly DriveService _driveService;
        private long _fileSize;
        private long _downloaded;

        public GoogleDriveApiManagerAdapter(string authfilePath,
            string directoryId,
            IFileAdapter fileAdapter,
            IFileInfoAdapter fileInfoAdapter)
        {
            _authfilePath = authfilePath;
            _directoryId = directoryId;
            _fileAdapter = fileAdapter;
            _fileInfoAdapter = fileInfoAdapter;
            _fileSize = 0;

            var credential = GoogleCredential.FromFile(_authfilePath)
                                    .CreateScoped(DriveService.ScopeConstants.Drive);

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        public async Task<string> UploadFileAsync(string filePath, Action<IUploadProgress> uploadProgress = null)
        {
            //Gathering file size at the very begining
            _fileSize = _fileInfoAdapter.FileSize(filePath);

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
                var request = _driveService.Files.Create(fileMetadata, fsSource, _fileAdapter.GetMimeType(filePath));
                request.Fields = "*";
                request.ChunkSize = 262144;
                if (uploadProgress != null)
                    request.ProgressChanged += uploadProgress;
                else
                    request.ProgressChanged += UploadProgress;

                var results = await request.UploadAsync(CancellationToken.None);

                if (results.Status == UploadStatus.Failed)
                    throw new Exception($"Error uploading file: {results.Exception.Message}");

                return request.ResponseBody?.Id;
            }
            else
                throw new Exception("Provide valid file path");
        }

        public async Task<GoogleDriveFiles> GetFilesAndFolders(string nextPageToken = null, FilesListOptionalParms optional = null)
        {

            return await Task.Run(() =>
            {
                try
                {
                    var files = new List<File>();

                        // Initial validation.
                        if (_driveService == null)
                            throw new ArgumentNullException("service");

                        //Providing default query parameter 'Q' to retrive only specific folder files
                        var defaultQueryPatter = $"'{_directoryId}' in parents";

                        if (!string.IsNullOrWhiteSpace(_directoryId))
                        {
                            if (optional == null)
                                optional = new FilesListOptionalParms
                                {
                                    Q = defaultQueryPatter
                                };
                            else if (optional != null && string.IsNullOrWhiteSpace(optional.Q))
                                optional.Q = defaultQueryPatter;
                        }

                        // Building the initial request.
                        var request = _driveService.Files.List();

                        // Applying optional parameters to the request.                
                        request = (FilesResource.ListRequest)ApplyOptionalParams(request, optional);

                        // Requesting data.
                        if (!string.IsNullOrWhiteSpace(nextPageToken))
                        request.PageToken = nextPageToken;

                    var fileFeed = request.Execute();

                    foreach (var item in fileFeed.Files)
                        files.Add(item);

                    var data = new GoogleDriveFiles
                    {
                        NextPageToken = fileFeed.NextPageToken,
                        Files = files
                    };

                    return data;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            });

        }

        public async Task DeleteAsync(string fileId, FilesDeleteOptionalParms optional = null)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Initial validation.
                    if (_driveService == null)
                        throw new ArgumentNullException("service");

                    if (string.IsNullOrWhiteSpace(fileId))
                        throw new ArgumentNullException("fileId");

                    // Building the initial request.
                    var request = _driveService.Files.Delete(fileId);

                    // Applying optional parameters to the request.                
                    request = (FilesResource.DeleteRequest)ApplyOptionalParams(request, optional);

                    // Requesting data.
                    request.Execute();
                }
                catch (Exception ex)
                {
                    throw new Exception("Request Files.Delete failed.", ex);
                }
            });
        }

        # region Private section
        private void UploadProgress(IUploadProgress progress)
        {
            _downloaded = 0;
            PrintProgressByPercentage(progress.BytesSent, _fileSize);
        }

        private void PrintProgressByPercentage(long progress, long total)
        {
            _downloaded += progress;
            Console.WriteLine($"Downloaded: {100 * _downloaded / total}%");

            if (_downloaded != total)
                ClearCurrentConsoleLine();
        }

        public void ClearCurrentConsoleLine()
        {
            if (Console.CursorTop > 0)
                Console.SetCursorPosition(0, Console.CursorTop - 1);

            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void DrawTextProgressBar(long progress, long total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }

        private object ApplyOptionalParams(object request, object optional)
        {
            if (optional == null)
                return request;

            System.Reflection.PropertyInfo[] optionalProperties = (optional.GetType()).GetProperties();

            foreach (System.Reflection.PropertyInfo property in optionalProperties)
            {
                // Copy value from optional parms to the request.  They should have the same names and datatypes.
                System.Reflection.PropertyInfo piShared = (request.GetType()).GetProperty(property.Name);
                if (property.GetValue(optional, null) != null) // TODO Test that we do not add values for items that are null
                    piShared.SetValue(request, property.GetValue(optional, null), null);
            }

            return request;
        }
        #endregion
    }

    public class GoogleDriveFiles
    {
        public string NextPageToken { get; set; }
        public IList<File> Files { get; set; }
    }

    public class FilesListOptionalParms
    {
        /// Comma-separated list of bodies of items (files/documents) to which the query applies. Supported bodies are 'user', 'domain', 'teamDrive' and 'allTeamDrives'. 'allTeamDrives' must be combined with 'user'; all other values must be used in isolation. Prefer 'user' or 'teamDrive' to 'allTeamDrives' for efficiency.
        public string Corpora { get; set; }
        /// The source of files to list. Deprecated: use 'corpora' instead.
        public string Corpus { get; set; }
        /// Whether Team Drive items should be included in results.
        public bool? IncludeTeamDriveItems { get; set; }
        /// A comma-separated list of sort keys. Valid keys are 'createdTime', 'folder', 'modifiedByMeTime', 'modifiedTime', 'name', 'name_natural', 'quotaBytesUsed', 'recency', 'sharedWithMeTime', 'starred', and 'viewedByMeTime'. Each key sorts ascending by default, but may be reversed with the 'desc' modifier. Example usage: ?orderBy=folder,modifiedTime desc,name. Please note that there is a current limitation for users with approximately one million files in which the requested sort order is ignored.
        public string OrderBy { get; set; }
        /// The maximum number of files to return per page. Partial or empty result pages are possible even before the end of the files list has been reached.
        public int? PageSize { get; set; }
        /// The token for continuing a previous list request on the next page. This should be set to the value of 'nextPageToken' from the previous response.
        public string PageToken { get; set; }
        /// A query for filtering the file results. See the "Search for Files" guide for supported syntax.
        public string Q { get; set; }
        /// A comma-separated list of spaces to query within the corpus. Supported values are 'drive', 'appDataFolder' and 'photos'.
        public string Spaces { get; set; }
        /// Whether the requesting application supports Team Drives.
        public bool? SupportsTeamDrives { get; set; }
        /// ID of Team Drive to search.
        public string TeamDriveId { get; set; }
        // Provide fields to retrive specified fields
        public string Fields { get; set; }
    }

    public class FilesDeleteOptionalParms
    {
        /// Whether the requesting application supports Team Drives.
        public bool? SupportsTeamDrives { get; set; }
    }
}
