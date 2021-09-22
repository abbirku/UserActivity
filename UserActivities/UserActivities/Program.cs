using Autofac;
using Infrastructure.Services;
using Infrastructure.ActiveProgram;
using Infrastructure.BrowserActivity;
using System.Threading.Tasks;
using CoreActivities.EgmaCV;
using CoreActivities.ScreenCapture;
using CoreActivities.RunningPrograms;
using CoreActivities.ActiveProgram;
using CoreActivities.BrowserActivity;
using CoreActivities.GoogleDriveApi;
using CoreActivities.FileManager;
using CoreActivities.DirectoryManager;

namespace UserActivities
{
    class Program
    {
        private static IContainer CompositionRoot()
        {
            var authFilePath = AppSettingsInfo.CreateGoogleDriveAuthFile(AppSettingsInfo.GetCurrentValue<string>("AuthFileName"));
            var directoryId = AppSettingsInfo.GetCurrentValue<string>("DirectoryId");

            var builder = new ContainerBuilder();

            //Package
            builder.RegisterType<Application>();
            builder.RegisterModule(new ActiveProgramPackage());
            builder.RegisterModule(new BrowserActivityPackage());
            builder.RegisterModule(new DirectoryManagerPackage());
            builder.RegisterModule(new EgmaCvPackage());
            builder.RegisterModule(new FileManagerPackage());
            builder.RegisterModule(new GoogleDriveApiPackage(authFilePath, directoryId));
            builder.RegisterModule(new RunningProgramPackage());
            builder.RegisterModule(new ScreenCapturePackage());

            //Service
            builder.RegisterType<ActiveProgramService>().As<IActiveProgramService>();
            builder.RegisterType<BrowserActivityService>().As<IBrowserActivityService>();
            builder.RegisterType<RunningProgramService>().As<IRunningProgramService>();
            builder.RegisterType<ScreenCaptureService>().As<IScreenCaptureService>();

            return builder.Build();
        }

        static async Task Main(string[] args)
        {
            await CompositionRoot().Resolve<Application>().Run();
        }
    }
}
