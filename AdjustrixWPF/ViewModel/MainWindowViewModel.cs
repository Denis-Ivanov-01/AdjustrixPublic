using System.Windows;
using System.Windows.Input;

namespace AdjustrixWPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public LanguageViewModel languageViewModel { get; set; }

        public ProjectFileViewModel projectViewModel { get; set; }

        public SystemFileManagement SystemFileSingleton { get; set; }

        public ProjectDataViewModel DataViewModel { get; set; }

        public DataLoadingViewModel DataLoadingViewModel { get; set; }

        public ProcessingViewModel ProcessingViewModel { get; set; }

        public MessageBoxViewModel MessageBoxViewModel { get; set; }

        public ICommand CloseCommand { get; }

        public MainWindowViewModel()
        {
            ProjectContainer projectContainer = new();
            MessageDelegate messageDelegate = new MessageDelegate();
            languageViewModel = LanguageViewModel.Singleton;
            projectViewModel = new ProjectFileViewModel(projectContainer, messageDelegate);
            DataViewModel = new(projectContainer);
            this.ProcessingViewModel = new ProcessingViewModel(projectContainer, messageDelegate);
            this.MessageBoxViewModel = new(messageDelegate);
            this.DataLoadingViewModel = new(projectContainer, messageDelegate);
            SystemFileSingleton = SystemFileManagement.Singleton;
            CloseCommand = new RelayCommand(Close);
        }

        private void Close(object param)
        {
            projectViewModel.PromptForUnsavedChangesAndSave();
            Application.Current.Shutdown();
        }
    }
}
