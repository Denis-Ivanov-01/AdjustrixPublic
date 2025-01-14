using Adjustment.Project;

namespace AdjustrixWPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AdjustrixProject _project;

        public LanguageViewModel languageViewModel { get; set; }

        public ProjectViewModel projectViewModel { get; set; }

        public SystemFileManagement SystemFileSingleton { get; set; }

        public MainWindowViewModel()
        {
            languageViewModel = new LanguageViewModel();
            projectViewModel = new ProjectViewModel();
            SystemFileSingleton = SystemFileManagement.Singleton;
        }
    }
}
