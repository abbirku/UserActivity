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
using CoreActivities.ActiveProgram;
using CoreActivities.BrowserActivity;

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
            builder.RegisterModule(new ActiveProgramPackage());
            builder.RegisterModule(new ActiveProgramPackage());
            builder.RegisterModule(new BrowserActivityPackage());

            //Adapter
            builder.RegisterType<DirectoryManagerAdapter>().As<IDirectoryManagerAdapter>();
            builder.RegisterType<FileAdapter>().As<IFileAdapter>();
            builder.RegisterType<FileStreamAdapter>().As<IFileStreamAdapter>();
            builder.RegisterType<FileInfoAdapter>().As<IFileInfoAdapter>();
            builder.RegisterType<GoogleDriveApiManagerAdapter>().As<IGoogleDriveApiManagerAdapter>()
                   .WithParameter("authfilePath", AppSettingsInfo.CreateGoogleDriveAuthFile(AppSettingsInfo.GetCurrentValue<string>("AuthFileName")))
                   .WithParameter("directoryId", AppSettingsInfo.GetCurrentValue<string>("DirectoryId"));

            //Service
            builder.RegisterType<RunningProgramService>().As<IRunningProgramService>();
            builder.RegisterType<ActiveProgramService>().As<IActiveProgramService>();
            builder.RegisterType<ScreenCaptureService>().As<IScreenCaptureService>();
            builder.RegisterType<BrowserActivityService>().As<IBrowserActivityService>();

            builder.RegisterType<DirectoryManagerService>().As<IDirectoryManagerService>();
            builder.RegisterType<FileManagerService>().As<IFileManagerService>();
            
            return builder.Build();
        }

        static async Task Main(string[] args)
        {
            await CompositionRoot().Resolve<Application>().Run();
        }
    }
}
