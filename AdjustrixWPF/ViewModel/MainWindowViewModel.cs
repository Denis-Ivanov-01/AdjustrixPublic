using Adjustment.Project;

namespace AdjustrixWPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public LanguageViewModel languageViewModel { get; set; }

        public ProjectViewModel projectViewModel { get; set; }

        public SystemFileManagement SystemFileSingleton { get; set; }

        public DataViewModel DataViewModel { get; set; }
        public MainWindowViewModel()
        {
            ProjectStore projectStore = new();
            languageViewModel = LanguageViewModel.Singleton;
            projectViewModel = new ProjectViewModel(projectStore);
            DataViewModel = new(projectStore);
            SystemFileSingleton = SystemFileManagement.Singleton;
        }
    }
}
