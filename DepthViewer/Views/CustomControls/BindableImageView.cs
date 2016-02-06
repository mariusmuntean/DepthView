using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
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

        #region constructors
        public BindableImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {   
            _imageLoader = ImageLoader.Instance;
        }

        public BindableImageView(Context context) : base(context)
        {
            _imageLoader = ImageLoader.Instance;
        }

        public BindableImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _imageLoader = ImageLoader.Instance;
        }

        public BindableImageView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            _imageLoader = ImageLoader.Instance;
        }

        public BindableImageView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            _imageLoader = ImageLoader.Instance;
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

        private void LoadImage()
        {
            // Prefix file path for UIL
            var newPath = "file://" + _path;

            Mvx.Resolve<IMvxMainThreadDispatcher>().RequestMainThreadAction(() =>
            {
               _imageLoader.DisplayImage(newPath, this);
            });

        }

        #endregion
    }
}