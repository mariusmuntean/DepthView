using Autofac;
using DepthViewer.Shared.Contracts;
using DepthViewer.Shared.Services;
using DepthViewerServer.Contracts;
using DepthViewerServer.Services;

namespace DepthViewerServer
{
    public class IoC
    {
        private static IContainer _container;

        public static void Initialize()
        {
            var cBuilder = new ContainerBuilder();

            // Register types and singletons here
            cBuilder.RegisterType<ParseConfig>().As<IParseConfig>();
            cBuilder.RegisterType<HangfireConfig>().As<IHangfireConfig>();
            cBuilder.RegisterType<ImageStitcher>().As<IImageStitcher>();
            cBuilder.RegisterType<ParseDataService>().As<IParseDataService>();

            _container = cBuilder.Build();

        }

        public static IContainer Container
        {
            get
            {
                return _container;
            }
        }
    }
}