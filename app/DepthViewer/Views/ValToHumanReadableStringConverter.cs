using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace DepthViewer.Android.Views
{
    public class ValToHumanReadableStringConverter:MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == 1? "Only one measurement": $"{value} measurements";
        }
    }
}