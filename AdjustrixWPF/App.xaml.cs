using System;
using System.Windows;
using AdjustrixWPF.Enums;
using AdjustrixWPF.SystemManagement;

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
        public static string InitializeFilePath { get; private set; } = string.Empty;

        public App()
        {
            language = (Language)Enum.Parse(typeof(Language), SystemFileManagement.Singleton.LanguageString);
            theme = (Theme)Enum.Parse(typeof(Theme), SystemFileManagement.Singleton.ThemeString);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                string filePath = e.Args[0];
                InitializeFilePath = filePath;
            }
            base.OnStartup(e);
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
