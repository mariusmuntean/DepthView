using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Platform.Converters;
using System.Globalization;

namespace DepthViewer.Views
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