using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Java.IO;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Debug = System.Diagnostics.Debug;
using Uri = Android.Net.Uri;

namespace DepthViewer.Views.CustomControls
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