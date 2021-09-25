using CoreActivities.BrowserActivity;
using CoreActivities.Extensions;
using CoreActivities.FileManager;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BrowserActivity
{
    public interface IBrowserActivityService
    {
        Task CaptureBrowserActivityAsync();
    }

    public class BrowserActivityService : IBrowserActivityService
    {
        private readonly IFile _file;
        private readonly IBrowserActivity _browserActivity;
        private readonly IConsoleHelper _consoleHelper;

        public BrowserActivityService(IFile file,
            IBrowserActivity browserActivity,
            IConsoleHelper consoleHelper)
        {
            _file = file;
            _browserActivity = browserActivity;
            _consoleHelper = consoleHelper;
        }

        public async Task CaptureBrowserActivityAsync()
        {
            Console.WriteLine("1. EnList Active Tab Url");
            Console.WriteLine("2. EnList Open Tabs");
            var option = Console.ReadLine();
            var result = string.Empty;

            if (option == "1")
            {
                result = _browserActivity.EnlistActiveTabUrl(BrowserType.Chrome);
                result.PrintResult();
            }
            else if (option == "2")
            {
                var allTabs = _browserActivity.EnlistAllOpenTabs(BrowserType.Chrome).OrderBy(x => x).ToList();
                result = allTabs.PrintResult();
            }
            else
                Console.WriteLine("Provide a valid option");

            await _consoleHelper.SaveResultToFileAsync(async (result, filePath) =>
            {
                await _file.AppendAllTextAsync(result, filePath);
            }, result);
        }
    }
}
