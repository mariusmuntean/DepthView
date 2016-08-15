using System;
using System.Globalization;
using Xamarin.Forms;

namespace DepthViewer.X.Converters
{
    public class CountToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var val = System.Convert.ToInt32(value);

            if (parameter == null)
            {
                return val == 0;
            }

            return val != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
