using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Platform.Converters;

namespace DepthViewer.Views
{
    public class ValToHumanReadableStringConverter:MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == 1? "Only one measurement": $"{value} measurements";
        }
    }
}