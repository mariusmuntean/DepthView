using Android.Content;
using Android.Graphics;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Cirrious.MvvmCross.Plugins.DownloadCache.Droid;
using Cirrious.MvvmCross.ViewModels;
using DepthViewer.Contracts;
using DepthViewer.Services;
using DepthViewer.Utils;
using UniversalImageLoader.Cache.Memory;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Assist;
using UniversalImageLoader.Utils;

namespace DepthViewer
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {

            // Set up UniversalImageLoader
            var options = new DisplayImageOptions.Builder()
                .CacheInMemory(true)
                .CacheOnDisk(false)
                .ConsiderExifParams(true)
                .ShowImageForEmptyUri(Resource.Drawable.splash)
                .ImageScaleType(ImageScaleType.InSampleInt)
                .BitmapConfig(Bitmap.Config.Rgb565)
                .ResetViewBeforeLoading(true)
                .Build();
            var uilConfig = new ImageLoaderConfiguration.Builder(applicationContext)
                .DefaultDisplayImageOptions(options)
                .Build();
            ImageLoader.Instance.Init(uilConfig);

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
            Mvx.LazyConstructAndRegisterSingleton<IDataExchangeService>(() => new DataExchangeService());
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Cirrious.MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded();

            var downdloadCacheConfig = new MvxDownloadCacheConfiguration();
            var fileDownloadCache = new CustomMvxFileDownloadCache(downdloadCacheConfig.CacheName, downdloadCacheConfig.CacheFolderPath, downdloadCacheConfig.MaxFiles, downdloadCacheConfig.MaxFileAge);

            Mvx.LazyConstructAndRegisterSingleton<IMvxFileDownloadCache>(() => fileDownloadCache);
        }
    }
}