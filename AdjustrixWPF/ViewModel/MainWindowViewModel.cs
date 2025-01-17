using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Adjustment.Project;

namespace AdjustrixWPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public LanguageViewModel languageViewModel { get; set; }

        public ProjectFileViewModel projectViewModel { get; set; }

        public SystemFileManagement SystemFileSingleton { get; set; }

        public ProjectDataViewModel DataViewModel { get; set; }
        
        public ICommand CloseCommand { get; }

        public MainWindowViewModel(params DependencyObject[] elements)
        {
            ProjectContainer projectStore = new();
            languageViewModel = LanguageViewModel.Singleton;
            projectViewModel = new ProjectFileViewModel(projectStore);
            DataViewModel = new(projectStore);
            SystemFileSingleton = SystemFileManagement.Singleton;
            CloseCommand = new RelayCommand(Close);
        }

        private void Close(object param)
        {
            projectViewModel.PromptSaveChanges();
            Application.Current.Shutdown();
        }
    }
}
