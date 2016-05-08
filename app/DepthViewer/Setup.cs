using Android.Content;
using Android.Graphics;
using DepthViewer.Contracts;
using DepthViewer.Services;
using DepthViewer.Utils;
using DepthViewer.Views.Presenter;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Assist;
using MvvmCross.Droid.Platform;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Plugins.DownloadCache;
using MvvmCross.Plugins.DownloadCache.Droid;
using MvxAndroidSetup = MvvmCross.Droid.Platform.MvxAndroidSetup;

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
				.ShowImageForEmptyUri(Resource.Drawable.ic_launcher)
                .ImageScaleType(ImageScaleType.InSampleInt)
                .BitmapConfig(Bitmap.Config.Rgb565)
                .ResetViewBeforeLoading(true)
                .Build();
            var uilConfig = new ImageLoaderConfiguration.Builder(applicationContext)
                .DefaultDisplayImageOptions(options)
                .Build();
            ImageLoader.Instance.Init(uilConfig);

        }

        protected override MvvmCross.Core.ViewModels.IMvxApplication CreateApp()
        {
            return new App();
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {

            var presenter = Mvx.IocConstruct<DepthViewerPresenter>();

            Mvx.RegisterSingleton<IMvxAndroidViewPresenter>(presenter);

            return presenter;
        }

        protected override MvvmCross.Platform.Platform.IMvxTrace CreateDebugTrace()
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
            Mvx.LazyConstructAndRegisterSingleton<IImageStitcher>(() => new RemoteImageStitcher());

            Mvx.ConstructAndRegisterSingleton<IFragmentTypeLookup, FragmentTypeLookup>();
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