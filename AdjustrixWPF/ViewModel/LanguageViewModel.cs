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
        private readonly string[] propertyNames;

        private static LanguageViewModel instance;
        public static LanguageViewModel Singleton => GetInstance();

        private readonly StringEntry file = new("Файл", "File");
        private readonly StringEntry data = new("Данни", "Data");
        private readonly StringEntry appearance = new("Изглед", "Appearance");
        private readonly StringEntry language = new("Език", "Language");
        private readonly StringEntry theme = new("Тема", "Theme");
        private readonly StringEntry newProject = new("Нов Проект", "New Project");
        private readonly StringEntry saveProject = new("Запази Проект", "Save Project");
        private readonly StringEntry openProject = new("Отвори Проект", "Open Project");
        private readonly StringEntry saveProjectQuestion = new("Искате ли да запазите промените по текущия проект?",
            "Do you want to save the changes made to current project?");
        private readonly StringEntry yes = new("Да", "Yes");
        private readonly StringEntry no = new("Не", "No");
        private readonly StringEntry chooseProjectFolder = new("Изберете папка за проекта...", "Choose a project folder...");
        private readonly StringEntry closeProjectQuestion = new("Искате ли да затворите текущия проект?", "Close the current project?");
        private readonly StringEntry projectName = new("Име на проект", "Project name");
        private readonly StringEntry siteName = new("Име на обект", "Site name");
        private readonly StringEntry contractor = new("Изпълнител", "Contractor");
        private readonly StringEntry client = new("Възложител", "Client");
        private readonly StringEntry projectType = new("Тип проект", "Project type");
        private readonly StringEntry fromPoint = new("Начална Точка", "From Point");
        private readonly StringEntry toPoint = new("Крайна Точка", "To Point");
        private readonly StringEntry length = new("Дължина", "Length");
        private readonly StringEntry value = new("Стойност", "Value");


        private static Language currentLanguage;

        private LanguageViewModel()
        {
            propertyNames = GetClassProperties();
            Languages = new();
            Languages.Add("Bulgarian");
            Languages.Add("English");
            currentLanguage = ParseLanguageString(SystemFileManagement.Singleton.LanguageString);
        }

        public static Language SelectedLanguage
        {
            get
            {
                return currentLanguage;
            }
            set
            {
                currentLanguage = value;
            }
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
            get { return theme.GetString(currentLanguage); }
        }

        public string SelectedTheme
        {
            get
            {
                return ThemeViewModel.SelectedTheme;
            }
            set
            {
                ThemeViewModel.SelectedTheme = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Themes
        {
            get
            {
                return ThemeViewModel.Singleton.Themes;
            }
            set
            {
                ThemeViewModel.Singleton.Themes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Languages { get; set; }

        public string NewProject
        {
            get
            {
                return newProject.GetString(currentLanguage);
            }
        }

        public string SaveProject
        {
            get
            {
                return saveProject.GetString(currentLanguage);
            }
        }

        public string OpenProject
        {
            get
            {
                return openProject.GetString(currentLanguage);
            }
        }

        public string SaveProjectQuestion
        {
            get
            {
                return saveProjectQuestion.GetString(currentLanguage);
            }
        }

        public string Yes
        {
            get
            {
                return yes.GetString(currentLanguage);
            }
        }

        public string No
        {
            get
            {
                return no.GetString(currentLanguage);
            }
        }


        public string ChooseProjectFolder
        {
            get
            {
                return chooseProjectFolder.GetString(currentLanguage);
            }
        }

        public string CloseProjectQuestion
        {
            get
            {
                return closeProjectQuestion.GetString(currentLanguage);
            }
        }

        public string ProjectName
        {
            get { return projectName.GetString(currentLanguage); }
        }

        public string SiteName
        {
            get { return siteName.GetString(currentLanguage); }
        }

        public string Contractor
        {
            get { return contractor.GetString(currentLanguage); }
        }

        public string Client
        {
            get { return client.GetString(currentLanguage); }
        }

        public string ProjectType
        {
            get { return projectType.GetString(currentLanguage); }
        }

        public string FromPoint
        {
            get
            {
                return fromPoint.GetString(currentLanguage);
            }
        }

        public string ToPoint
        {
            get
            {
                return toPoint.GetString(currentLanguage);
            }
        }

        public string Length
        {
            get
            {
                return length.GetString(currentLanguage);
            }
        }

        public string Value
        {
            get
            {
                return value.GetString(currentLanguage);
            }
        }
        private static LanguageViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new LanguageViewModel();
            }
            return instance;
        }

        private static string[] GetClassProperties()
        {
            Type type = typeof(LanguageViewModel);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string[] result = new string[properties.Length];
            for (int i = 0; i < properties.Length; i++)
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

        private static Language ParseLanguageString(string language)
        {
            if (Enum.TryParse(language, out Language lang))
            {
                return lang;
            }
            throw new ArgumentException("Incorrect enum string value was passed!");
        }
    }
}
