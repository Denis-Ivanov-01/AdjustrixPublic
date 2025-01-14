using System.Windows.Input;
using Adjustment.Project;
using Microsoft.Win32;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectViewModel : ViewModelBase
    {
        private bool projectLoaded = false;
        private AdjustrixProject project;
        private ProjectFile projectFile;
        private float saveProjectOpacity = 0.0f;

        public ICommand OpenProject { get; }

        public ICommand SaveProject { get; }

        public ICommand CreateProject { get; }

        public ProjectViewModel()
        {
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
            projectFile.ToFile(project);
        }

        private bool CanSaveProject(object parameter)
        {
            return project != null;
        }
    }
}
