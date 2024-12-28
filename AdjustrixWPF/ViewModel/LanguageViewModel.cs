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
        private StringEntry dark = new("Тъмна", "Dark");
        private StringEntry light = new("Светла", "Light");

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
            currentLanguage = Language.English;
            selectedTheme = Theme.Dark.ToString();
        }

        public string CurrentLanguage
        {
            get
            {
                return currentLanguage.ToString();
            }
            set
            {
                currentLanguage = Language.English.ToString() == value ? Language.English : Language.Bulgarian;

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
                return selectedTheme;
            }
            set
            {
                selectedTheme = value;
                if (value == Theme.Dark.ToString())
                {
                    ThemeViewModel.SetDarkTheme();
                }
                else
                {
                    ThemeViewModel.SetLightTheme();
                }
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
    }
}
