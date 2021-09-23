﻿using CoreActivities.DirectoryManager;
using CoreActivities.FileManager;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IConsoleHelper
    {
        Task SaveResultToFileAsync<T>(Func<T, string, Task> method, T result);
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

        public async Task SaveResultToFileAsync<T>(Func<T, string, Task> method, T result)
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
                        await method(result, filePath);
                        break;
                    }
                }
            }
            else
                return;
        }
    }
}
