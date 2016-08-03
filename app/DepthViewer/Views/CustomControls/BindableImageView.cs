using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;
using MvvmCross.Plugins.DownloadCache;
using UniversalImageLoader.Core;

namespace DepthViewer.Android.Views.CustomControls
{
    public class BindableImageView : ImageView
    {
        private string _path;
        private ImageLoader _imageLoader;
        private IMvxFileDownloadCache _downloadCache;

        #region constructors
        public BindableImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {   
            _imageLoader = ImageLoader.Instance;
            _downloadCache = Mvx.Resolve<IMvxFileDownloadCache>();
        }

        public BindableImageView(Context context) : base(context)
        {
            _imageLoader = ImageLoader.Instance;
            _downloadCache = Mvx.Resolve<IMvxFileDownloadCache>();
        }

        public BindableImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _imageLoader = ImageLoader.Instance;
            _downloadCache = Mvx.Resolve<IMvxFileDownloadCache>();
        }

        public BindableImageView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            _imageLoader = ImageLoader.Instance;
            _downloadCache = Mvx.Resolve<IMvxFileDownloadCache>();
        }

        public BindableImageView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            _imageLoader = ImageLoader.Instance;
            _downloadCache = Mvx.Resolve<IMvxFileDownloadCache>();
        }

        #endregion constructors

        #region BindableProperties

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;

                LoadImage();
            }
        }

        private async void LoadImage()
        {
            var baseFilesDir = Application.Context.FilesDir.Path;
            var tcs = new TaskCompletionSource<string>();
            _downloadCache.RequestLocalFilePath(_path, s =>
            {
                tcs.SetResult(s);
            }, exception =>
            {
                tcs.SetException(exception);
            });

            if (tcs.Task.IsFaulted || tcs.Task.IsCanceled)
            {
                return;
            }


            // Prefix file path for UIL
            var cachedPath = await tcs.Task;
            var newPath = "file://" + System.IO.Path.Combine(baseFilesDir, cachedPath);

            Mvx.Resolve<IMvxMainThreadDispatcher>().RequestMainThreadAction(() =>
            {
               _imageLoader.DisplayImage(newPath, this);
            });

        }

        #endregion
    }
}