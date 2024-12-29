using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace AdjustrixWPF.ViewModel
{
    internal class StringEntry
    {
        public string Bulgarian { get; set; }
        public string English { get; set; }
        public StringEntry(string bg, string en)
        {
            Bulgarian = bg;
            English = en;
        }

        public string GetString(Language currentLanguage)
        {
            if (currentLanguage == Language.English)
            {
                return English;
            }
            return Bulgarian;
        }
    }

    public class LanguageViewModel : ViewModelBase
    {
        private string[] propertyNames;

        private StringEntry file = new("Файл", "File");
        private StringEntry data = new("Данни", "Data");
        private StringEntry appearance = new("Изглед", "Appearance");
        private StringEntry language = new("Език", "Language");
        private StringEntry theme = new("Тема", "Theme");
        
        private string selectedTheme;
        private ObservableCollection<string> themes = EnumHelper.GetEnumStrings<Theme>();

        public static Language currentLanguage;

        public LanguageViewModel()
        {
            propertyNames = GetClassProperties();
            Languages = new();
            Languages.Add("Bulgarian");
            Languages.Add("English");
            //todo: later load it from AppData/Local
            currentLanguage = ParseLanguageString(SystemFileManagement.Singleton.LanguageString);
            selectedTheme = ThemeViewModel.Singleton.SelectedTheme;
        }

        public string CurrentLanguage
        {
            get
            {
                return currentLanguage.ToString();
            }
            set
            {
                currentLanguage = ParseLanguageString(value);
                SystemFileManagement.Singleton.LanguageString = value;
                OnPropertiesChanged();
            }
        }

        public string File
        {
            get { return file.GetString(currentLanguage); }
        }

        public string Data
        {
            get { return data.GetString(currentLanguage); }
        }

        public string Appearance
        {
            get { return appearance.GetString(currentLanguage); }
        }

        public string LanguageString
        {
            get { return language.GetString(currentLanguage); }
        }

        public string ThemeString
        {
            get { return theme.GetString(currentLanguage);}
        }

        public string SelectedTheme
        {
            get
            {
                return ThemeViewModel.Singleton.SelectedTheme;
            }
            set
            {
                ThemeViewModel.Singleton.SelectedTheme = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Themes
        {
            get { return themes; }
            set
            {
                themes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Languages { get; set; }

        private string[] GetClassProperties()
        {
            Type type = typeof(LanguageViewModel);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string[] result = new string[properties.Length];
            for (int i=0;i<properties.Length;i++)
            {
                result[i] = properties[i].Name;
            }
            return result;
        }

        private void OnPropertiesChanged()
        {
            foreach (string prop in propertyNames)
            {
                if (prop != nameof(Languages))
                {
                    OnPropertyChanged(prop);
                }
            }
        }

        private Language ParseLanguageString(string language)
        {
            if (Enum.TryParse(language, out Language lang))
            {
                return lang;
            }
            throw new ArgumentException("Incorrect enum string value was passed!");
        }
    }
}
