﻿using Microsoft.Extensions.Configuration;
using System.IO;

namespace WebCamImageSync
{
    /// <summary>
    /// Note: While adding appsetting.json in console app, goto properties and select "Copy if newer" for "Copy to Output Directory" option.
    /// Otherwise it will throw exception
    /// </summary>
    public static class AppSettingsInfo
    {
        public static string ConfigFileName { get; set; } = "appsettings.json";

        // Get a valued stored in the appsettings.
        // Pass in a key like TestArea:TestKey to get Value
        public static T GetCurrentValue<T>(string Key)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile(ConfigFileName, optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            return configuration.GetValue<T>(Key);
        }
    }
}