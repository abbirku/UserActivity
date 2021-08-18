using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Automation;

namespace Infrastructure.BrowserActivity
{
    public interface IBrowserActivityAdapter
    {
        IList<string> GetOpenTabsInfos(string browserName);
        string GetActiveTabUrl(string browserName);
    }

    public class BrowserActivityAdapter : IBrowserActivityAdapter
    {
        public string GetActiveTabUrl(string browserName)
        {
            try
            {
                // There are always multiple chrome processes, so we have to loop through all of them to find the
                // process with a Window Handle and an automation element of name "Address and search bar"

                var browserProcess = Process.GetProcessesByName(browserName);
                if (browserProcess.Length > 0)
                {
                    var url = string.Empty;
                    foreach (var chrome in browserProcess)
                    {
                        // The browser process must have a window
                        if (chrome.MainWindowHandle == IntPtr.Zero)
                            continue;

                        // Find the automation element
                        var elm = AutomationElement.FromHandle(chrome.MainWindowHandle);
                        var elmUrlBar = elm.FindFirst(TreeScope.Descendants,
                            new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

                        // If it can be found, get the value from the URL bar
                        if (elmUrlBar != null)
                        {
                            var patterns = elmUrlBar.GetSupportedPatterns();
                            if (patterns.Length > 0)
                            {
                                var val = (ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0]);
                                url = val.Current.Value;
                            }
                        }
                    }

                    return url;
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<string> GetOpenTabsInfos(string browserName)
        {
            try
            {
                var tabInfos = new List<string>();

                if (string.IsNullOrWhiteSpace(browserName))
                    throw new Exception("Provide valid browser name");

                var chromeProcesses = Process.GetProcessesByName("chrome");
                if (chromeProcesses.Length <= 0)
                    throw new Exception($"No process found with this {browserName} name");
                else
                {
                    foreach (Process proc in chromeProcesses)
                    {
                        if (proc.MainWindowHandle != IntPtr.Zero)
                        {
                            var root = AutomationElement.FromHandle(proc.MainWindowHandle);
                            var condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
                            var tabs = root.FindAll(TreeScope.Descendants, condition);
                            var enumerator = tabs.GetEnumerator();

                            while (enumerator.MoveNext())
                            {
                                var info = (AutomationElement)enumerator.Current;
                                tabInfos.Add(info.Current.Name);
                            }
                        }
                    }
                }

                return tabInfos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
