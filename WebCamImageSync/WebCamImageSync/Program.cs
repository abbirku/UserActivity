using Autofac;
using Infrastructure.DirectoryManager;
using Infrastructure.EgmaCV;
using Infrastructure.FileManager;
using System;

namespace WebCamImageSync
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

            return builder.Build();
        }

        static void Main(string[] args)
        {
            CompositionRoot().Resolve<Application>().Run();
        }
    }
}
