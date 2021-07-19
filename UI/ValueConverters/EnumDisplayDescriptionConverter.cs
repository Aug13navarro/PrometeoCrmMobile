using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace UI.ValueConverters
{
    public class EnumDisplayDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string description = GetAttributeOfType<DescriptionAttribute>((Enum)value).Description;
            return "description";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        private T GetAttributeOfType<T>(Enum enumVal) where T : System.Attribute
        {
            Type type = enumVal.GetType();
            MemberInfo[] memInfo = type.GetMember(enumVal.ToString());
            object[] attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }
}
