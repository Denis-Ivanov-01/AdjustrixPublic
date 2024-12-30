namespace AdjustrixWPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public LanguageViewModel languageViewModel { get; set; }

        public MainWindowViewModel()
        {
            languageViewModel = new LanguageViewModel();
        }
    }
}
