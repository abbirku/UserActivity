using CoreActivities.RunningPrograms;
using Infrastructure.FileManager;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IRunningProgramService
    {
        Task CaptureProgramTitleAsync(string filePath);
        Task CaptureProcessNameAsync(string filePath);
    }

    public class RunningProgramService : IRunningProgramService
    {
        private readonly IFileAdapter _fileAdapter;
        private readonly IRunningPrograms _runningPrograms;

        public RunningProgramService(IFileAdapter fileAdapter,
            IRunningPrograms runningPrograms)
        {
            _fileAdapter = fileAdapter;
            _runningPrograms = runningPrograms;
        }

        public async Task CaptureProcessNameAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt"))
                throw new Exception("Provide a valid txt file");

            var processes = _runningPrograms.GetRunningProcessList().Select((x, index) =>
            {
                return $"{index + 1}. {x}";
            }).ToList();

            await _fileAdapter.AppendAllLineAsync(processes, filePath);
        }

        public async Task CaptureProgramTitleAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt"))
                throw new Exception("Provide a valid txt file");

            var programTitles = _runningPrograms.GetRunningProgramsList().Select((x, index) =>
            {
                return $"{index + 1}. {x}";
            }).ToList();

            await _fileAdapter.AppendAllLineAsync(programTitles, filePath);
        }
    }
}
