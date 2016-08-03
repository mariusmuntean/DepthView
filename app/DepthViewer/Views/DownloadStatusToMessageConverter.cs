using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace DepthViewer.Android.Views
{
    public class DownloadStatusToMessageConverter: MvxValueConverter<bool, string>
    {
		public DownloadStatusToMessageConverter()
		{
			
		}
        protected override string Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? "Downloaded" : "Download";
        }
    }
}