namespace AdjustrixWPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        public ThemeViewModel themeViewModel { get; set; }

        public LanguageViewModel languageViewModel { get; set; }

        public MainWindowViewModel()
        {
            themeViewModel = new ThemeViewModel();
            languageViewModel = new LanguageViewModel();
        }
    }
}
