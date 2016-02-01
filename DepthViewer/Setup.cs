using Android.Content;
using Android.Views;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.CrossCore.Plugins;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Cirrious.MvvmCross.Plugins.DownloadCache.Droid;
using Cirrious.MvvmCross.ViewModels;
using DepthViewer.Contracts;
using DepthViewer.Services;

namespace DepthViewer
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
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
            Mvx.LazyConstructAndRegisterSingleton<IParseConfig>(() => new ParseConfig());
            Mvx.LazyConstructAndRegisterSingleton<ILocalMappingServices>(() => new LocalMappingService());
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Cirrious.MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.DownloadCache.PluginLoader.Instance.EnsureLoaded();

            var downdloadCacheConfig = new MvxDownloadCacheConfiguration();
            var fileDownloadCache = new MvxFileDownloadCache(downdloadCacheConfig.CacheName, downdloadCacheConfig.CacheFolderPath, downdloadCacheConfig.MaxFiles, downdloadCacheConfig.MaxFileAge);

            Mvx.LazyConstructAndRegisterSingleton<IMvxFileDownloadCache>(() => fileDownloadCache);
        }
    }
}