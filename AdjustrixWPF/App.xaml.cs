using System;
using System.Windows;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    ///

    public partial class App : Application
    {
        private static Language language;
        private static Theme theme;

        public App()
        {
            language = (Language)Enum.Parse(typeof(Language), SystemFileManagement.Singleton.LanguageString);
            theme = (Theme)Enum.Parse(typeof(Theme), SystemFileManagement.Singleton.ThemeString);
        }

        public static Language Language
        {
            get { return language; }
            set
            {
                language = value;
            }
        }

        public static Theme Theme
        {
            get
            {
                return theme;
            }
            set
            {
                theme = value;
            }
        }
    }
}
