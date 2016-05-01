using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Platform.Converters;

namespace DepthViewer.Views
{
    public class DownloadStatusToResourceConverter : MvxValueConverter<bool, int>
    {
        protected override int Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? Resource.Drawable.checkCircle : Resource.Drawable.cloudDownload;
        }
    }
}