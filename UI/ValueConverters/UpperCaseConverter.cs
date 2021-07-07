using System;
using System.Globalization;
using Xamarin.Forms;

namespace UI.ValueConverters
{
    public class UpperCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueStr = (string)value;

            if (string.IsNullOrWhiteSpace(valueStr))
            {
                return "";
            }

            return valueStr.ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
