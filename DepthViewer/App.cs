using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using DepthViewer.Contracts;
using DepthViewer.ViewModels;
using Parse;

namespace DepthViewer
{
    public class App : Cirrious.MvvmCross.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<FirstViewModel>();

            InitialiseParse();
        }

        private void InitialiseParse()
        {
            var parseConfig = Mvx.Resolve<IParseConfig>();
            ParseClient.Initialize(parseConfig.ApplicationId, parseConfig.DotNetKey);
        }
    }
}