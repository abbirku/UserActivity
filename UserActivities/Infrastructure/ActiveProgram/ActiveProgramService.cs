using Infrastructure.FileManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ActiveProgram
{
    public interface IActiveProgramService
    {
        Task CaptureActiveProgramTitleAsync(string filePath);
    }

    public class ActiveProgramService : IActiveProgramService
    {
        private readonly IFileAdapter _fileAdapter;
        private readonly IActiveProgramAdapter _activeProgramAdapter;

        public ActiveProgramService(IFileAdapter fileAdapter,
            IActiveProgramAdapter activeProgramAdapter)
        {
            _fileAdapter = fileAdapter;
            _activeProgramAdapter = activeProgramAdapter;
        }

        public async Task CaptureActiveProgramTitleAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt"))
                throw new Exception("Provide a valid txt file");

            var windowTitle = _activeProgramAdapter.GetActiveWindowTitle();
            var parts = windowTitle.Split("\\");

            await _fileAdapter.AppendAllTextAsync(parts[^1], filePath);
        }
    }
}
