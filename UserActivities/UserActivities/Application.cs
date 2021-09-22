using Infrastructure.Services;
using Infrastructure.ActiveProgram;
using System;
using System.Threading.Tasks;
using Infrastructure.BrowserActivity;
using CoreActivities.EgmaCV;
using CoreActivities.DirectoryManager;
using CoreActivities.GoogleDriveApi;
using CoreActivities.ActiveProgram;
using System.Collections;
using System.Collections.Generic;
using CoreActivities.BrowserActivity;
using CoreActivities.RunningPrograms;
using System.Linq;

namespace UserActivities
{
    public class Application
    {
        private readonly IEgmaCv _egmaCv;
        private readonly IDirectoryManager _directoryManagerService;
        private readonly IGoogleDriveApiManager _googleDriveApiManagerAdapter;
        private readonly IScreenCaptureService _screenCaptureService;
        private readonly IRunningPrograms _runningProgram;
        private readonly IActiveProgram _activeProgram;
        private readonly IBrowserActivity _browserActivity;
        private string _folderName;

        public Application(IEgmaCv egmaCv,
            IScreenCaptureService screenCaptureService,
            IRunningPrograms runningProgram,
            IDirectoryManager directoryManagerService,
            IGoogleDriveApiManager googleDriveApiManagerAdapter,
            IActiveProgram activeProgram,
            IBrowserActivity browserActivity)
        {
            _egmaCv = egmaCv;
            _screenCaptureService = screenCaptureService;
            _runningProgram = runningProgram;
            _directoryManagerService = directoryManagerService;
            _folderName = AppSettingsInfo.GetCurrentValue<string>("FolderName");
            _googleDriveApiManagerAdapter = googleDriveApiManagerAdapter;
            _activeProgram = activeProgram;
            _browserActivity = browserActivity;
        }

        public async Task Run()
        {
            try
            {
                Console.WriteLine("Starting User Activities...");
                await Task.Run(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("Select an active (Provide number as input)");
                        Console.WriteLine("1. Active Program");
                        Console.WriteLine("2. Browser Activity");
                        Console.WriteLine("3. Running Programs");

                        var option = Console.ReadLine();
                        if (option == "1")
                        {
                            var title = _activeProgram.CaptureActiveProgramTitle();
                            PrintResult(title);
                        }
                        else if (option == "2")
                        {
                            Console.WriteLine("1. EnList Active Tab Url");
                            Console.WriteLine("2. EnList Open Tabs");
                            option = Console.ReadLine();

                            if (option == "1")
                            {
                                var activeTabUrl = _browserActivity.EnlistActiveTabUrl(BrowserType.Chrome);
                                PrintResult(activeTabUrl);
                            }
                            else if (option == "2")
                            {
                                var allTabs = _browserActivity.EnlistAllOpenTabs(BrowserType.Chrome).OrderBy(x => x).ToList();
                                PrintResult(allTabs);
                            }
                            else
                            {
                                Console.WriteLine("Provide a valid option number");
                            }
                        }
                        else if (option == "3")
                        {
                            Console.WriteLine("1. Programs");
                            Console.WriteLine("2. Processes");
                            option = Console.ReadLine();

                            if (option == "1")
                            {
                                var results = _runningProgram.GetRunningProgramsList().OrderBy(x => x).ToList();
                                PrintResult(results);
                            }
                            else if (option == "2")
                            {
                                var results = _runningProgram.GetRunningProcessList().OrderBy(x => x).ToList();
                                PrintResult(results);
                            }
                            else
                            {
                                Console.WriteLine("Provide a valid option number");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Provide a valid option number");
                        }

                        ClearScreen();
                    }
                });
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        private void PrintResult(string result)
        {
            Console.WriteLine(result);
        }

        private void PrintResult(IList<string> results)
        {
            var tupleList = new List<(string, string, string)>();
            var main = results.Count - (results.Count % 3);
            var left = results.Count % 3;

            if (main != 0)
            {
                for (int i = 0; i < main; i += 3)
                    tupleList.Add((results[i], results[i + 1], results[i + 2]));
            }

            if (left != 0 && left == 1)
                tupleList.Add((results[main], string.Empty, string.Empty));
            else if (left != 0 && left == 2)
                tupleList.Add((results[main], results[main + 1], string.Empty));
            else if (left != 0 && left == 3)
                tupleList.Add((results[main], results[main + 1], results[main + 2]));

            Console.WriteLine(tupleList.AsEnumerable().ToStringTable(
                    new[] { "", "", "" },
                    a => a.Item1, a => a.Item2, a => a.Item3));

            Console.WriteLine();
        }

        private void ClearScreen()
        {
            Console.WriteLine("Clear Screen? (y/n)");
            var option = Console.ReadLine();
            if (option == "y")
                Console.Clear();
        }
    }
}
