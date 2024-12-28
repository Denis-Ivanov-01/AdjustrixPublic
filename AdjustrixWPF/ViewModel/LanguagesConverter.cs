using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace AdjustrixWPF.ViewModel
{
    public class LanguagesConverter : IValueConverter
    {
        private const string englishString = "English (EN)";
        private const string bulgarianString = "Български (BG)";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<string> languages)
            {
                return ConvertLanguages(languages);
            }
            else if (value is string language)
            {
                return ConvertLanguage(language);
            }
            return value;
        }

        private string ConvertLanguage(string language)
        {
            if (language == Language.English.ToString())
            {
                return englishString;
            }
            return bulgarianString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<string>)
            {
                return ConvertLanguagesBack((ObservableCollection<string>)value);
            }
            else if (value is string stringValue)
            {
                return ConvertLanguageBack(stringValue);
            }
            return Binding.DoNothing;
        }

        private ObservableCollection<string> ConvertLanguages(IEnumerable<string> languages)
        {
            ObservableCollection<string> result = new();
            foreach (string language in languages)
            {
                result.Add(ConvertLanguage(language));
            }
            return result;
        }

        public ObservableCollection<string> ConvertLanguagesBack(IEnumerable<string> strings)
        {
            ObservableCollection<string> result = new();
            foreach(string language in strings)
            {
                result.Add(ConvertLanguageBack(language));
            }
            return result;
        }

        private string ConvertLanguageBack(string langString)
        {
            if (langString == bulgarianString)
            {
                return Language.Bulgarian.ToString();
            }
            return Language.English.ToString();
        }
    }
}
