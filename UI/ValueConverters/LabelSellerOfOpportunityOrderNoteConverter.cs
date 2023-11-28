using Core.Model;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace UI.ValueConverters
{
    public class LabelSellerOfOpportunityOrderNoteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var data = value as User;
                return data.CodeFullName;
            }

            return "Sin Vendedor";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
