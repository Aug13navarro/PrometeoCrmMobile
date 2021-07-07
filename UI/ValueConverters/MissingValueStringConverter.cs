using System;
using System.Globalization;
using Xamarin.Forms;

namespace UI.ValueConverters
{
    public class MissingValueStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
            {
                return "N/E";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
