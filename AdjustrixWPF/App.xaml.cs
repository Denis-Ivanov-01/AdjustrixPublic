using System.Windows;

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
            //todo: replace these with reading from AppData
            language = Language.English;
            theme = Theme.Dark;
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
