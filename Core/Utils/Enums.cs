using System;
using System.ComponentModel;

namespace Core.Utils
{
    public static class Enums
    {
        public static string GetEnumDescription(this Enum value)
        {
            var entries = value.ToString().Split(',');
            var description = new string[entries.Length];

            for (var i = 0; i < entries.Length; i++)
            {
                var fieldInfo = value.GetType().GetField(entries[i].Trim());
                var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                description[i] = (attributes.Length > 0) ? attributes[0].Description : entries[i].Trim();
            }

            return string.Join(", ", description);
        }
    }
}
