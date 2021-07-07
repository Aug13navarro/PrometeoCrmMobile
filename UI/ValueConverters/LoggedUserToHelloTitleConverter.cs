using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UI.LangResources;
using Xamarin.Forms;

namespace UI.ValueConverters
{
    public class LoggedUserToHelloTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = AppResources.Hello + ", " + value;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
