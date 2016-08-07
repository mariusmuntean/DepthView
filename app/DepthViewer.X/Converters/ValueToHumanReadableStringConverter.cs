using System;
using System.Globalization;
using DepthViewer.Core.Converters;
using Xamarin.Forms;

namespace DepthViewer.X.Converters
{
    public class ValueToHumanReadableStringConverter: IValueConverter
    {
        private ValToHumanReadableStringConverter converter = new ValToHumanReadableStringConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return converter.Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
