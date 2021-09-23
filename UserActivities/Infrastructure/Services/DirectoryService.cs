using CoreActivities.DirectoryManager;
using CoreActivities.Extensions;
using System.Linq;

namespace Infrastructure.Services
{
    public interface IDirectoryService
    {
        void CaptureDirectoryActivity();
    }

    public class DirectoryService : IDirectoryService
    {
        private readonly IDirectoryManager _directoryManager;

        public DirectoryService(IDirectoryManager directoryManager)
        {
            _directoryManager = directoryManager;
        }

        public void CaptureDirectoryActivity()
        {
            var fileList = _directoryManager.ListFilesInDirectory(_directoryManager.GetProgramDataDirectoryPath(Constants.WorkingFolder));
            
            fileList = fileList.Select(x =>
            {
                var parts = x.Split("\\");
                var res = parts.Count() > 0 ? parts.ElementAt(parts.Count() - 1) : string.Empty;
                return res;
            }).ToList();

            fileList.PrintResult();
        }
    }
}
