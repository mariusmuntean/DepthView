using DepthViewer.Contracts;
using DepthViewer.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using Parse;

namespace DepthViewer
{
    public class App : MvxApplication
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