using CoreActivities.ActiveProgram;
using CoreActivities.DirectoryManager;
using CoreActivities.Extensions;
using CoreActivities.FileManager;
using Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Infrastructure.ActiveProgram
{
    public interface IActiveProgramService
    {
        Task CaptureActiveProgramActivityAsync();
    }

    public class ActiveProgramService : IActiveProgramService
    {
        private readonly IFile _file;
        private readonly IActiveProgram _activeProgram;
        private readonly IConsoleHelper _consoleHelper;

        public ActiveProgramService(IFile file,IActiveProgram activeProgram,
            IConsoleHelper consoleHelper)
        {
            _file = file;
            _activeProgram = activeProgram;
            _consoleHelper = consoleHelper;
        }

        public async Task CaptureActiveProgramActivityAsync()
        {
            var title = _activeProgram.CaptureActiveProgramTitle();
            var parts = title.Split("\\");
            parts[^1].PrintResult();

            await _consoleHelper.SaveResultToFileAsync(async (title, filePath) =>
            {
                await _file.AppendAllTextAsync(title, filePath);
            }, parts[^1]);
        }
    }
}
