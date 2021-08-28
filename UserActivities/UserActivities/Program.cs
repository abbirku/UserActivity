using Autofac;
using Infrastructure.DirectoryManager;
using Infrastructure.FileManager;
using Infrastructure.GoogleDriveApi;
using Infrastructure.Services;
using Infrastructure.ActiveProgram;
using Infrastructure.BrowserActivity;
using System;
using System.Threading.Tasks;
using CoreActivities.EgmaCV;
using CoreActivities.ScreenCapture;
using CoreActivities.RunningPrograms;

namespace UserActivities
{
    class Program
    {
        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();
            
            //Package
            builder.RegisterType<Application>();
            builder.RegisterModule(new EgmaCvPackage());
            builder.RegisterModule(new ScreenCapturePackage());
            builder.RegisterModule(new RunningProgramPackage());

            //Adapter
            builder.RegisterType<DirectoryManagerAdapter>().As<IDirectoryManagerAdapter>();
            builder.RegisterType<FileAdapter>().As<IFileAdapter>();
            builder.RegisterType<FileStreamAdapter>().As<IFileStreamAdapter>();
            builder.RegisterType<ActiveProgramAdapter>().As<IActiveProgramAdapter>();
            builder.RegisterType<BrowserActivityAdapter>().As<IBrowserActivityAdapter>();
            builder.RegisterType<FileInfoAdapter>().As<IFileInfoAdapter>();
            builder.RegisterType<GoogleDriveApiManagerAdapter>().As<IGoogleDriveApiManagerAdapter>()
                   .WithParameter("authfilePath", AppSettingsInfo.CreateGoogleDriveAuthFile(AppSettingsInfo.GetCurrentValue<string>("AuthFileName")))
                   .WithParameter("directoryId", AppSettingsInfo.GetCurrentValue<string>("DirectoryId"));

            //Service
            builder.RegisterType<DirectoryManagerService>().As<IDirectoryManagerService>();
            builder.RegisterType<FileManagerService>().As<IFileManagerService>();
            builder.RegisterType<ScreenCaptureService>().As<IScreenCaptureService>();
            builder.RegisterType<RunningProgramService>().As<IRunningProgramService>();
            builder.RegisterType<ActiveProgramService>().As<IActiveProgramService>();
            builder.RegisterType<BrowserActivityService>().As<IBrowserActivityService>();
            
            return builder.Build();
        }

        static async Task Main(string[] args)
        {
            await CompositionRoot().Resolve<Application>().Run();
        }
    }
}
