using Autofac;
using Infrastructure.DirectoryManager;
using Infrastructure.EgmaCV;
using Infrastructure.FileManager;
using Infrastructure.GoogleDriveApi;
using Infrastructure.ScreenCapture;
using System;
using System.Threading.Tasks;

namespace UserActivities
{
    class Program
    {
        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterType<Application>();
            builder.RegisterType<DirectoryManagerAdapter>().As<IDirectoryManagerAdapter>();
            builder.RegisterType<DirectoryManagerService>().As<IDirectoryManagerService>();
            builder.RegisterType<FileAdapter>().As<IFileAdapter>();
            builder.RegisterType<FileStreamAdapter>().As<IFileStreamAdapter>();
            builder.RegisterType<FileManagerService>().As<IFileManagerService>();
            builder.RegisterType<EgmaCvAdapter>().As<IEgmaCvAdapter>();
            builder.RegisterType<ScreenCaptureAdapter>().As<IScreenCaptureAdapter>();
            builder.RegisterType<ScreenCaptureService>().As<IScreenCaptureService>();
            builder.RegisterType<GoogleDriveApiManagerAdapter>().As<IGoogleDriveApiManagerAdapter>()
                   .WithParameter("authfilePath", AppSettingsInfo.CreateGoogleDriveAuthFile(AppSettingsInfo.GetCurrentValue<string>("AuthFileName")))
                   .WithParameter("directoryId", AppSettingsInfo.GetCurrentValue<string>("DirectoryId")); ;

            return builder.Build();
        }

        static async Task Main(string[] args)
        {
            await CompositionRoot().Resolve<Application>().Run();
        }
    }
}
