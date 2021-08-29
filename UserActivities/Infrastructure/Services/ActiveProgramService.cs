using CoreActivities.ActiveProgram;
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
        private readonly IActiveProgram _activeProgram;

        public ActiveProgramService(IFileAdapter fileAdapter,
            IActiveProgram activeProgram)
        {
            _fileAdapter = fileAdapter;
            _activeProgram = activeProgram;
        }

        public async Task CaptureActiveProgramTitleAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt"))
                throw new Exception("Provide a valid txt file");

            var windowTitle = _activeProgram.CaptureActiveProgramTitle();
            var parts = windowTitle.Split("\\");

            await _fileAdapter.AppendAllTextAsync(parts[^1], filePath);
        }
    }
}
