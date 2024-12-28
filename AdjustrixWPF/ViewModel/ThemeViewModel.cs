using System;
using System.Collections.ObjectModel;
using System.Windows;
using AdjustrixWPF.View;


namespace AdjustrixWPF.ViewModel
{
    public class ThemeViewModel : ViewModelBase
    {
        private string theme;
        private ObservableCollection<string> themes = EnumHelper.GetEnumStrings<Theme>();

        public ThemeViewModel()
        {
            theme = App.Theme.ToString();
        }

        public string SelectedTheme 
        { 
            get 
            {
                return theme; 
            } 
            set 
            { 
                theme = value; 
                OnPropertyChanged();
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
                OnPropertyChanged(); 
            }
        }

        private static void SetDarkTheme()
        {

            ThemeManager.SetCurrentThemeDictionary(Application.Current.MainWindow, new Uri(@"pack://application:,,,/View/Resources/DarkTheme.xaml"));
        }

        private static void SetLightTheme()
        {
            ThemeManager.SetCurrentThemeDictionary(Application.Current.MainWindow, new Uri(@"pack://application:,,,/View/Resources/LightTheme.xaml"));
        }
    }
}
