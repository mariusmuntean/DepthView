using System;
using System.Globalization;
using Xamarin.Forms;

namespace DepthViewer.X.Converters
{
    public class DownloadStatusToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && (bool)value ? "Downloaded" : "Download";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
