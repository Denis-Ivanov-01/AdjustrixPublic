using System.Windows.Input;
using Adjustment.Adjustment.Leveling;
using Adjustment.Project;

namespace AdjustrixWPF.ViewModel
{
    public class ProcessingViewModel : ViewModelBase
    {
        private ProjectContainer projectContainer;
        private AdjustrixProject currentProject;

        public ICommand Adjust { get; }

        public ProcessingViewModel(ProjectContainer projectContainer)
        {
            this.projectContainer = projectContainer;
            projectContainer.ProjectChanged += OnProjectChanged;
            Adjust = new RelayCommand(PerformProcessing, CanProcess);
        }

        private void OnProjectChanged(AdjustrixProject project)
        {
            currentProject = project;
        }

        private void PerformProcessing(object param)
        {
            LevelingProject project = (LevelingProject)currentProject;
            LevelingProcessing processing = new(project.HeightDifferences);
            processing.PerformAdjustment();
        }
        
        private bool CanProcess(object param)
        {
            return currentProject != null;
        }
    }
}
