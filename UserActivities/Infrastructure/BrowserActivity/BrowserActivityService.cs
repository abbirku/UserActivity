using Infrastructure.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BrowserActivity
{
    public interface IBrowserActivityService
    {
        Task EnlistAllOpenTabs(string browserName, string filePath);
        Task EnlistActiveTabUrl(string browserName, string filePath);
    }

    public class BrowserActivityService : IBrowserActivityService
    {
        private readonly IFileAdapter _fileAdapter;
        private readonly IBrowserActivityAdapter _browserActivityAdapter;

        public BrowserActivityService(IFileAdapter fileAdapter,
            IBrowserActivityAdapter browserActivityAdapter)
        {
            _fileAdapter = fileAdapter;
            _browserActivityAdapter = browserActivityAdapter;
        }

        public async Task EnlistActiveTabUrl(string browserName, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt") || string.IsNullOrEmpty(browserName))
                throw new Exception("Provide a valid txt file and browser name");

            var url = _browserActivityAdapter.GetActiveTabUrl(browserName);
            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"No URL found for {browserName} browser");

            var parts = url.Split("/");
            var validUrl = string.Empty;

            if (parts.Length > 0)
                validUrl = parts[0];

            if (string.IsNullOrWhiteSpace(validUrl))
                throw new Exception($"No valid url found on active tab for browser {browserName}");

            await _fileAdapter.AppendAllTextAsync(validUrl, filePath);
        }

        public async Task EnlistAllOpenTabs(string browserName, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt") || string.IsNullOrEmpty(browserName))
                throw new Exception("Provide a valid txt file and browser name");

            var tabs = _browserActivityAdapter.GetOpenTabsInfos(browserName).Select((x, index) =>
            {
                return $"{index + 1}. {x}";
            }).ToList();

            await _fileAdapter.AppendAllLineAsync(tabs, filePath);
        }
    }
}
