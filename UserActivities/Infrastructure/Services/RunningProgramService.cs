using CoreActivities.RunningPrograms;
using CoreActivities.FileManager;
using System;
using System.Linq;
using System.Threading.Tasks;
using CoreActivities.Extensions;

namespace Infrastructure.Services
{
    public interface IRunningProgramService
    {
        Task CaptureRunningPrgramActivityAsync();
    }

    public class RunningProgramService : IRunningProgramService
    {
        private readonly IFile _file;
        private readonly IRunningPrograms _runningPrograms;
        private readonly IConsoleHelper _consoleHelper;

        public RunningProgramService(IFile file,
            IRunningPrograms runningPrograms,
            IConsoleHelper consoleHelper)
        {
            _file = file;
            _runningPrograms = runningPrograms;
            _consoleHelper = consoleHelper;
        }

        public async Task CaptureRunningPrgramActivityAsync()
        {
            Console.WriteLine("1. Programs");
            Console.WriteLine("2. Processes");
            var option = Console.ReadLine();
            var result = string.Empty;

            if (option == "1")
            {
                var results = _runningPrograms.GetRunningProgramsList().OrderBy(x => x).ToList();
                result = results.PrintResult();
            }
            else if (option == "2")
            {
                var results = _runningPrograms.GetRunningProcessList().OrderBy(x => x).ToList();
                result = results.PrintResult();
            }
            else
                Console.WriteLine("Provide a valid option number");

            await _consoleHelper.SaveResultToFileAsync(async (result, filePath) =>
            {
                await _file.AppendAllTextAsync(result, filePath);
            }, result);
        }
    }
}
