using System.Windows.Input;
using Adjustment.Project;
using Microsoft.Win32;
using AdjustrixWPF.View.UserControls;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectViewModel : ViewModelBase
    {
        private bool projectLoaded = false;
        private bool projectChanged = false;
        private bool projectSaved = true;
        private AdjustrixProject project;
        private ProjectFile projectFile;

        //private float saveProjectOpacity = 0.0f;

        private LanguageViewModel languageViewModel;

        public LanguageViewModel LanguageViewModel 
        {
            get 
            {
                return languageViewModel;
            }
             
        }

        public ICommand OpenProject { get; }

        public ICommand SaveProject { get; }

        public ICommand CreateProject { get; }

        public ProjectViewModel()
        {
            languageViewModel = LanguageViewModel.Singleton;
            projectFile = new ProjectFile();
            OpenProject = new RelayCommand(OpenProjectFile, CanOpenProject);
            SaveProject = new RelayCommand(SaveProjectFile, CanSaveProject);
        }


        public bool ProjectLoaded
        {
            get { return projectLoaded; }
            set
            {
                projectLoaded = value;
                OnPropertyChanged();
            }
        }

        private void OpenProjectFile(object parameter)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Adjustrix project file (.adjx)|*.adjx";
            dialog.DefaultExt = ".adjx";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                project = projectFile.FromFile(dialog.FileName);
                ProjectLoaded = true;
            }
        }

        private bool CanOpenProject(object parameter)
        {
            return true;
        }

        private void SaveProjectFile(object parameter)
        {
            if (projectChanged)
            {
                bool result = AskProjectSave();
                if (result == false) { return; }
            }
            
            projectFile.ToFile(project);
        }

        private bool CanSaveProject(object parameter)
        {
            return project != null;
        }

        private bool AskProjectSave()
        {
            SaveProjectPrompt dialog = new(this);
            dialog.ShowDialog();
            return dialog.SaveProject;
        }
    }
}
