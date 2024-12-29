namespace AdjustrixWPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        public ThemeViewModel themeViewModel { get; set; }

        public LanguageViewModel languageViewModel { get; set; }

        public MainWindowViewModel()
        {
            themeViewModel = ThemeViewModel.Singleton;
            languageViewModel = new LanguageViewModel();
        }
    }
}
