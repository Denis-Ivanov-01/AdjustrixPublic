using System.Windows.Input;
using System.Diagnostics;
using Adjustment;
using Adjustment.Adjustment.Leveling;
using Adjustment.Project;
using AdjustrixWPF.View.UserControls;
using System;

namespace AdjustrixWPF.ViewModel
{
    public class ProcessingViewModel : ViewModelBase
    {
        private ProjectContainer projectContainer;
        private AdjustrixProject currentProject;
        private string projectFolder;

        public ICommand Adjust { get; }

        public ProcessingViewModel(ProjectContainer projectContainer)
        {
            this.projectContainer = projectContainer;
            this.projectContainer.ProjectChanged += OnProjectChanged;
            this.projectContainer.ProjectFolderChanged += OnProjectFolderChanged;
            Adjust = new RelayCommand(PerformProcessing, CanProcess);
        }

        private void OnProjectFolderChanged(string obj)
        {
            projectFolder = obj;
        }

        private void OnProjectChanged(AdjustrixProject project)
        {
            currentProject = project;
        }

        private void PerformProcessing(object param)
        {
             if (string.IsNullOrWhiteSpace(projectFolder)) 
            {
                //todo: error messages (prompts)
            }
            try
            {
                LevelingProject project = (LevelingProject)currentProject;
                LevelingProcessing processing = new(project.HeightDifferences);
                processing.Process(projectFolder);
            }
            catch (Exception ex)
            {
                ProcessingErrorPrompt p = new(ex.Message);
                p.ShowDialog();
                return;
            }
            SuccessfulProcessingPrompt prompt = new();
            prompt.ShowDialog();
            if (prompt.RevealReports) 
            {
                Process.Start("explorer.exe", projectFolder);
            }
        }
        
        private bool CanProcess(object param)
        {
            return currentProject != null;
        }
    }
}
