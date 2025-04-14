using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using AdjustrixWPF.Enums;
using AdjustrixWPF.SystemManagement;
using AdjustrixWPF.View;
using MaterialDesignThemes.Wpf;


namespace AdjustrixWPF.ViewModel
{
    public class ThemeViewModel
    {
        private ObservableCollection<string> themes = EnumHelper.GetEnumStrings<Enums.Theme>();


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
                if (value == Enums.Theme.Dark.ToString())
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

        public static void SetDarkTheme()
        {
            Color color = (Color)ColorConverter.ConvertFromString("#2196f3");
            SetMaterialDesignTheme(color, color, BaseTheme.Dark);
            ThemeManager.SetCurrentThemeDictionary(Application.Current.MainWindow, new Uri(@"pack://application:,,,/View/Resources/DarkTheme.xaml"));
        }

        public static void SetLightTheme()
        {
            Color color = (Color)ColorConverter.ConvertFromString("#2196f3");
            SetMaterialDesignTheme(color, Colors.White, BaseTheme.Light);
            ThemeManager.SetCurrentThemeDictionary(Application.Current.MainWindow, new Uri(@"pack://application:,,,/View/Resources/LightTheme.xaml"));
        }

        private static void SetMaterialDesignTheme(Color primary, Color secondary, BaseTheme baseTheme)
        {
            PaletteHelper helper = new();
            var theme = helper.GetTheme();
            theme.SetPrimaryColor(primary);
            theme.SetSecondaryColor(secondary);
            theme.SetBaseTheme(baseTheme);
            helper.SetTheme(theme);
        }
    }
}
