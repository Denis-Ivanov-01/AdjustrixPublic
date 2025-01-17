using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AdjustrixWPF.View;
using MaterialDesignThemes.Wpf;
using md = MaterialDesignThemes.Wpf;


namespace AdjustrixWPF.ViewModel
{
    public class ThemeViewModel
    {
        private ObservableCollection<string> themes = EnumHelper.GetEnumStrings<Theme>();
        private static List<DependencyObject> objects = new();

        private static md.BundledTheme darkTheme = new md.BundledTheme
        {
            PrimaryColor = MaterialDesignColors.PrimaryColor.Blue,
            BaseTheme = md.BaseTheme.Dark,
            SecondaryColor = MaterialDesignColors.SecondaryColor.LightBlue,
            ColorAdjustment = new md.ColorAdjustment()
        };

        public static md.BundledTheme lightTheme = new md.BundledTheme
        {
            PrimaryColor = MaterialDesignColors.PrimaryColor.Blue,
            SecondaryColor = MaterialDesignColors.SecondaryColor.DeepPurple,
            BaseTheme = md.BaseTheme.Light,
            ColorAdjustment = new()
        };

        private static ThemeViewModel instance;

        public static ThemeViewModel Singleton => GetInstance();

        private ThemeViewModel()
        {
            // Must be set manually when creating the instance in order to replace the ResourceDictionary if needed
            SelectedTheme = SystemFileManagement.Singleton.ThemeString;
        }

        private static ThemeViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new ThemeViewModel();
            }
            return instance;
        }

        public static string SelectedTheme
        {
            get
            {
                return SystemFileManagement.Singleton.ThemeString;
            }
            set
            {
                SystemFileManagement.Singleton.ThemeString = value;
                if (value == Theme.Dark.ToString())
                {
                    SetDarkTheme();
                }
                else
                {
                    SetLightTheme();
                }
            }
        }

        public ObservableCollection<string> Themes
        {
            get { return themes; }
            set
            {
                themes = value;
            }
        }

        public static void RegisterObject(DependencyObject obj)
        {
            objects.Add(obj);
        }

        public static void SetDarkTheme()
        {
            foreach (DependencyObject obj in objects)
            {
                ColorZoneAssist.SetMode(obj, ColorZoneMode.Dark);
            }
            md.ThemeAssist.ChangeTheme(darkTheme, md.BaseTheme.Dark);
            ThemeManager.SetCurrentThemeDictionary(Application.Current.MainWindow, new Uri(@"pack://application:,,,/View/Resources/DarkTheme.xaml"));
        }

        public static void SetLightTheme()
        {
            foreach (DependencyObject obj in objects)
            {
                ColorZoneAssist.SetMode(obj, ColorZoneMode.Light);
            }
            md.ThemeAssist.ChangeTheme(lightTheme, md.BaseTheme.Light);
            ThemeManager.SetCurrentThemeDictionary(Application.Current.MainWindow, new Uri(@"pack://application:,,,/View/Resources/LightTheme.xaml"));
        }
    }

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
