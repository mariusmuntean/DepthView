using DepthViewer.Core.Contracts;
using DepthViewer.Core.Services;
using DepthViewer.Core.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Presenter.Core;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace DepthViewer.X
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.RegisterType<IMvxFormsPageLoader, DepthViewerViewLoader>();
            Mvx.Resolve<IParseDataService>().InitializeParse();
            Mvx.LazyConstructAndRegisterSingleton<IDataExchangeService>(() => new DataExchangeService());

            RegisterAppStart<LocalMappingsViewModel>();
        }
    }
}
