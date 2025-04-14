using System.Windows;
using System.Windows.Input;
using AdjustrixWPF.Commands;
using AdjustrixWPF.Containers;
using AdjustrixWPF.SystemManagement;

namespace AdjustrixWPF.ViewModel
{
    public class MainWindowViewModel : AdjustrixViewModel
    {
        public ProjectFileViewModel ProjectViewModel { get; set; }

        public SystemFileManagement SystemFileSingleton { get; set; }

        public ProjectDataViewModel DataViewModel { get; set; }

        public DataLoadingViewModel DataLoadingViewModel { get; set; }

        public ImportScriptsViewModel ImportScriptsViewModel { get; set; }

        public ProcessingViewModel ProcessingViewModel { get; set; }

        public MessageBoxViewModel MessageBoxViewModel { get; set; }

        public ICommand CloseCommand { get; }

        public MainWindowViewModel()
        {
            ProjectContainer projectContainer = new();
            MessageDelegate messageDelegate = new MessageDelegate();
            //todo: dependency injection
            //todo: event aggregator
            //todo: unit testing for viewmodels
            //a good idea - viewmodels and user controls in a separate project
            //todo: check for click-once deploy
            DataViewModel = new(projectContainer);
            this.ProcessingViewModel = new ProcessingViewModel(projectContainer, messageDelegate);
            this.MessageBoxViewModel = new(messageDelegate);
            this.DataLoadingViewModel = new(projectContainer, messageDelegate);
            this.ImportScriptsViewModel = new(projectContainer, messageDelegate);
            SystemFileSingleton = SystemFileManagement.Singleton;
            CloseCommand = new RelayCommand(Close);

            //This ViewModel must be initialized last, because it checks if the program was opened
            //by opening a project file. If it was, it will load the project.
            //So the other ViewModels must be initialized to be subscribed to the projectContainer
            //events and perform the according actions.
            ProjectViewModel = new ProjectFileViewModel(projectContainer, messageDelegate);
        }

        private void Close(object param)
        {
            ProjectViewModel.HandleUnsavedChanges();
            Application.Current.Shutdown();
        }
    }
}
