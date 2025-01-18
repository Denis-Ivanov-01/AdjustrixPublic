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

        public ProcessingViewModel ProcessingViewModel { get; set; }

        public MessageBoxViewModel MessageBoxViewModel { get; set; }
        
        public ICommand CloseCommand { get; }

        public MainWindowViewModel()
        {
            ProjectContainer projectContainer = new();
            MessageDelegate messageDelegate = new MessageDelegate();
            languageViewModel = LanguageViewModel.Singleton;
            projectViewModel = new ProjectFileViewModel(projectContainer);
            DataViewModel = new(projectContainer);
            this.ProcessingViewModel = new ProcessingViewModel(projectContainer, messageDelegate);
            this.MessageBoxViewModel = new(messageDelegate);
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
