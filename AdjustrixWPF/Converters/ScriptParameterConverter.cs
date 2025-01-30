using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using AdjustrixWPF.Enums;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.Converters
{
    public class ScriptParameterConverter : IValueConverter
    {

        private const string fileEN = "File";
        private const string fileBG = "Файл";

        private const string dirEN = "Directory";
        private const string dirBG = "Директория";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            // Handle individual enum value conversion
            if (value is ScriptParameterType enumValue)
            {
                return EnumToString(enumValue);
            }

            if (value is string enumStr)
            {
                return EnumToString(enumStr);
            }

            // Handle collection of enum values
            if (value is IEnumerable enumCollection /*&& targetType == typeof(IEnumerable<string>)*/)
            {
                return enumCollection.Cast<string>().Select(e => EnumToString(e)).ToList();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            // Convert string back to enum value
            if (value is string stringValue && targetType.IsEnum)
            {
                return StringToEnum(stringValue);
            }

            // Convert collection of strings back to collection of enums
            if (value is IEnumerable<string> stringCollection && targetType.IsAssignableFrom(typeof(IEnumerable)))
            {
                var enumType = targetType.GenericTypeArguments.FirstOrDefault();
                if (enumType != null && enumType.IsEnum)
                {
                    return stringCollection.Select(s => StringToEnum(s)).ToList();
                }
            }

            return value;
        }

        private string EnumToString(ScriptParameterType type)
        {
            if (LanguageViewModel.SelectedLanguage == Language.English)
            {
                if (type == ScriptParameterType.Directory)
                {
                    return dirEN;
                }
                return fileEN;
            }
            if (type == ScriptParameterType.Directory)
            {
                return "Директория";
            }
            return "Файл";
        }


        private string EnumToString(string type)
        {
            if (LanguageViewModel.SelectedLanguage == Language.English)
            {
                if (type == ScriptParameterType.Directory.ToString())
                {
                    return dirEN;
                }
                return fileEN;
            }
            if (type == ScriptParameterType.Directory.ToString())
            {
                return "Директория";
            }
            return "Файл";
        }

        private ScriptParameterType StringToEnum(string type)
        {
            if (LanguageViewModel.SelectedLanguage == Language.English)
            {
                if (type == fileEN) return ScriptParameterType.FilePath;
                else if (type == dirEN) return ScriptParameterType.Directory;
                throw new NotImplementedException();
            }
            else
            {
                if (type == fileBG) return ScriptParameterType.FilePath;
                else if (type == dirBG) return ScriptParameterType.Directory;
                throw new NotImplementedException();
            }
        }
    }
}
