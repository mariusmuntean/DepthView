using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace DepthViewer.Android.Views
{
    public class DownloadStatusToResourceConverter : MvxValueConverter<bool, int>
    {
        protected override int Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? Resource.Drawable.checkCircle : Resource.Drawable.cloudDownload;
        }
    }
}