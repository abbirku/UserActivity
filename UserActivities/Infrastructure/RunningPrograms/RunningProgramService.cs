using Infrastructure.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.RunningPrograms
{
    public interface IRunningProgramService
    {
        Task CaptureProgramTitleAsync(string filePath);
        Task CaptureProcessNameAsync(string filePath);
    }

    public class RunningProgramService : IRunningProgramService
    {
        private readonly IFileAdapter _fileAdapter;
        private readonly IRunningProgramAdapter _runningProgramAdapter;

        public RunningProgramService(IFileAdapter fileAdapter,
            IRunningProgramAdapter runningProgramAdapter)
        {
            _fileAdapter = fileAdapter;
            _runningProgramAdapter = runningProgramAdapter;
        }

        public async Task CaptureProcessNameAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt"))
                throw new Exception("Provide a valid txt file");

            var processes = _runningProgramAdapter.GetRunningProcessList().Select((x, index) =>
            {
                return $"{index + 1}. {x}";
            }).ToList();

            await _fileAdapter.AppendAllLineAsync(processes, filePath);
        }

        public async Task CaptureProgramTitleAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt"))
                throw new Exception("Provide a valid txt file");

            var programTitles = _runningProgramAdapter.GetRunningProgramsList().Select((x, index) =>
            {
                return $"{index + 1}. {x}";
            }).ToList();

            await _fileAdapter.AppendAllLineAsync(programTitles, filePath);
        }
    }
}
