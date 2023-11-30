using Core.Model;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace UI.ValueConverters
{
    public class ColorStatusOfOpportunityOrderNoteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (StatusOrderNote)value;
            if (status == null) return string.Empty;
            return status.ColorHexa;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
