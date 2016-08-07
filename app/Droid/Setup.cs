using Android.Content;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Core.Views;
using DepthViewer.Core.Contracts;
using DepthViewer.Core.Services;
using DepthViewer.Android.Services;
using DepthViewer.Android.Models;
using Acr.UserDialogs;
using DepthViewer.Utils;
using MvvmCross.Plugins.DownloadCache;
using MvvmCross.Plugins.DownloadCache.Droid;

namespace DepthViewer.X.Droid
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            var presenter = new DepthViewerViewPresenter();
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);
            return presenter;
        }

        protected override IMvxApplication CreateApp()
        {
            return new App();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

protected override void InitializeIoC()
        {
            base.InitializeIoC();

            Mvx.LazyConstructAndRegisterSingleton<IParseDataService, ParseDataService>();
            Mvx.LazyConstructAndRegisterSingleton<IPathProvider, PathProvider>();
            Mvx.LazyConstructAndRegisterSingleton<IParseConfig>(() => new ParseConfig());
            Mvx.LazyConstructAndRegisterSingleton<ILocalMappingServices>(() => new LocalMappingService());
            Mvx.LazyConstructAndRegisterSingleton<IDataExchangeService>(() => new DataExchangeService());
            Mvx.LazyConstructAndRegisterSingleton<IImageStitcher>(() => new RemoteImageStitcher());
            Mvx.LazyConstructAndRegisterSingleton<ISecureDataStore, SecureDataStore>();

            Mvx.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
            MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded();

            var downdloadCacheConfig = new MvxDownloadCacheConfiguration();
            var fileDownloadCache = new CustomMvxFileDownloadCache(downdloadCacheConfig.CacheName, downdloadCacheConfig.CacheFolderPath, downdloadCacheConfig.MaxFiles, downdloadCacheConfig.MaxFileAge);

            Mvx.LazyConstructAndRegisterSingleton<IMvxFileDownloadCache>(() => fileDownloadCache);
            Mvx.LazyConstructAndRegisterSingleton<IDownloadCache>(() => fileDownloadCache);
        }
    }
}
