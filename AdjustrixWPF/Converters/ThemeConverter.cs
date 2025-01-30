using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using AdjustrixWPF.Enums;
using AdjustrixWPF.ViewModel;


namespace AdjustrixWPF.Converters
{
    public class ThemeConverter : IValueConverter
    {
        private const string darkBg = "Тъмна";
        private const string lightBg = "Светла";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is IEnumerable<string> themes)
            {
                ObservableCollection<string> result = new();
                foreach (string theme in themes)
                {
                    result.Add(ConvertTheme(theme));
                }
                return result;
            }
            else if (value is string theme)
            {
                return ConvertTheme(theme);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> themeStrings)
            {
                ObservableCollection<string> result = new();
                foreach (string str in themeStrings)
                {
                    result.Add(ConvertThemeBack(str));
                }
                return result;
            }
            else if (value is string themeStr)
            {
                return ConvertThemeBack(themeStr);
            }
            return Binding.DoNothing;
        }

        private static string ConvertTheme(string theme)
        {
            if (LanguageViewModel.SelectedLanguage == Language.English)
            {
                return theme;
            }
            if (theme == Theme.Dark.ToString())
            {
                return darkBg;
            }
            return lightBg;
        }

        private static string ConvertThemeBack(string langStr)
        {
            if (langStr == Theme.Dark.ToString() || langStr == darkBg)
            {
                return Theme.Dark.ToString();
            }
            return Theme.Light.ToString();
        }
    }
}
