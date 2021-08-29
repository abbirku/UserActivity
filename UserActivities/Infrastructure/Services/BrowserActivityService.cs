using CoreActivities.BrowserActivity;
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
        Task EnlistAllOpenTabs(BrowserType browserType, string filePath);
        Task EnlistActiveTabUrl(BrowserType browserType, string filePath);
    }

    public class BrowserActivityService : IBrowserActivityService
    {
        private readonly IFileAdapter _fileAdapter;
        private readonly IBrowserActivity _browserActivity;
        private readonly BrowserActivityEnumAdaptee _browserActivityEnumAdaptee;

        public BrowserActivityService(IFileAdapter fileAdapter,
            IBrowserActivity browserActivity,
            BrowserActivityEnumAdaptee browserActivityEnumAdaptee)
        {
            _fileAdapter = fileAdapter;
            _browserActivity = browserActivity;
            _browserActivityEnumAdaptee = browserActivityEnumAdaptee;
        }

        public async Task EnlistActiveTabUrl(BrowserType browserType, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt"))
                throw new Exception("Provide a valid txt file and browser name");

            var url = _browserActivity.EnlistActiveTabUrl(browserType);
            var parts = url.Split("/");
            var validUrl = string.Empty;

            if (parts.Length > 0)
                validUrl = parts[0];

            if (string.IsNullOrWhiteSpace(validUrl))
                throw new Exception($"No valid url found on active tab for browser {_browserActivityEnumAdaptee.ToDescriptionString(browserType)}");

            await _fileAdapter.AppendAllTextAsync(validUrl, filePath);
        }

        public async Task EnlistAllOpenTabs(BrowserType browserType, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains("txt"))
                throw new Exception("Provide a valid txt file and browser name");

            var tabs = _browserActivity.EnlistAllOpenTabs(browserType).Select((x, index) =>
            {
                return $"{index + 1}. {x}";
            }).ToList();

            await _fileAdapter.AppendAllLineAsync(tabs, filePath);
        }
    }
}
