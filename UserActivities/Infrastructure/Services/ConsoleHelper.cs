using CoreActivities.DirectoryManager;
using CoreActivities.FileManager;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IConsoleHelper
    {
        Task SaveResultToFileAsync<T>(Func<T, string, Task> method, T result);
        Task SaveResultToFileAsync(Func<string, Task> method);
        void SaveResultToFile<T>(Action<T, string> method, T result);
    }

    public class ConsoleHelper : IConsoleHelper
    {
        private readonly IFile _file;
        private readonly IDirectoryManager _directoryManager;

        public ConsoleHelper(IFile file, IDirectoryManager directoryManager)
        {
            _file = file;
            _directoryManager = directoryManager;
        }

        private async Task SavePatternAsync(Func<string, Task> commonMethod)
        {
            Console.WriteLine("Want to save the result? (y/n)");
            var option = Console.ReadLine();
            if (option == "y")
            {
                while (true)
                {
                    Console.WriteLine("Provide a file name");
                    var fileName = Console.ReadLine();

                    var filePath = _directoryManager.CreateProgramDataFilePath(Constants.WorkingFolder, fileName);
                    if (_file.DoesExists(filePath))
                    {
                        Console.WriteLine($"{fileName} already exits");
                        Console.WriteLine("Want to continue? (y/n)");
                        option = Console.ReadLine();
                        if (option == "y")
                            continue;
                        else
                            break;
                    }
                    else
                    {
                        _file.CreateFile(filePath);
                        await commonMethod(filePath);
                        break;
                    }
                }
            }
            else if (option == "n")
                return;
            else
                Console.WriteLine("Provide a valid option");
        }

        public async Task SaveResultToFileAsync<T>(Func<T, string, Task> method, T result)
        {
            await SavePatternAsync(async (filePath) =>
            {
                await method(result, filePath);
            });
        }

        public async Task SaveResultToFileAsync(Func<string, Task> method)
        {
            await SavePatternAsync(async (filePath) =>
            {
                await method(filePath);
            });
        }

        public void SaveResultToFile<T>(Action<T, string> method, T result)
        {
            SavePatternAsync(async (filePath) =>
            {
                await Task.Run(() =>
                {
                    method(result, filePath);
                });
            }).Wait();
        }
    }
}
