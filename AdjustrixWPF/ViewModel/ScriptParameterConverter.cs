using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AdjustrixWPF.ViewModel
{
    public class ScriptParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // Handle individual enum value conversion
            if (value is Enum enumValue)
            {
                return enumValue.ToString();
            }

            // Handle collection of enum values
            if (value is IEnumerable enumCollection && targetType == typeof(IEnumerable<string>))
            {
                return enumCollection.Cast<Enum>().Select(e => e.ToString()).ToList();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // Convert string back to enum value
            if (value is string stringValue && targetType.IsEnum)
            {
                return Enum.Parse(targetType, stringValue);
            }

            // Convert collection of strings back to collection of enums
            if (value is IEnumerable<string> stringCollection && targetType.IsAssignableFrom(typeof(IEnumerable)))
            {
                var enumType = targetType.GenericTypeArguments.FirstOrDefault();
                if (enumType != null && enumType.IsEnum)
                {
                    return stringCollection.Select(s => Enum.Parse(enumType, s)).ToList();
                }
            }

            return value;
        }
    }
}
