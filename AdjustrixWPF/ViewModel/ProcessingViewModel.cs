using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Adjustment;
using Adjustment.Extensions;
using Adjustment.Project;
using AdjustrixWPF.View.UserControls;

namespace AdjustrixWPF.ViewModel
{
    public class ProcessingViewModel : ViewModelBase
    {
        private readonly ProjectContainer projectContainer;
        private AdjustrixProject currentProject;
        private string projectFolder;

        private AdjustmentStatus status;
        private readonly AdjustmentStatusDelegate statusDelegate;

        private readonly MessageDelegate messageDelegate;

        public ICommand Adjust { get; }

        public ProcessingViewModel(ProjectContainer projectContainer,
            MessageDelegate messageDelegate)
        {
            this.projectContainer = projectContainer;
            this.messageDelegate = messageDelegate;
            this.projectContainer.ProjectChanged += OnProjectChanged;
            this.projectContainer.ProjectFolderChanged += OnProjectFolderChanged;
            Adjust = new RelayCommand(PerformProcessing, CanProcess);
            statusDelegate = new();
            statusDelegate.StatusChanged += OnAdjustmentStatusChanged;
            statusDelegate.DurationChanged += OnProcessingDurationChanged;
        }

        private void OnProcessingDurationChanged(TimeSpan obj)
        {
            StatusMessage = GenerateStatusMessage(obj);
            messageDelegate.ChangeMessage(StatusMessage, TimeSpan.FromSeconds(5));
        }

        private void OnAdjustmentStatusChanged(AdjustmentStatus status)
        {
            this.status = status;
            StatusMessage = GenerateStatusMessage();
            messageDelegate.ChangeMessage(StatusMessage);
        }

        private void OnProjectFolderChanged(string obj)
        {
            projectFolder = obj;
        }

        private void OnProjectChanged(AdjustrixProject project)
        {
            currentProject = project;
        }

        private async void PerformProcessing(object param)
        {
            if (string.IsNullOrWhiteSpace(projectFolder))
            {
                //todo: error messages (prompts)
            }
            try
            {
                LevelingProject project = (LevelingProject)currentProject;
                await Task.Run(() =>
                {
                    LevelingProcessing processing = new(project, statusDelegate);
                    processing.Process(projectFolder);
                });
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

        private string statusMessage;

        public string StatusMessage
        {
            get { return statusMessage; }
            set { statusMessage = value; }
        }

        private string GenerateStatusMessage()
        {
            string message;
            switch (status)
            {
                case AdjustmentStatus.DoingNothing:
                    message = "";
                    break;
                case AdjustmentStatus.PreparingNetworkData:
                    message = "Preparing the network data...";
                    break;
                case AdjustmentStatus.AnalyzingNetwork:
                    message = "Performing geometric network analysis...";
                    break;
                case AdjustmentStatus.CalculatingAdjustment:
                    message = "Adjusting the network...";
                    break;
                case AdjustmentStatus.CreatingReports:
                    message = "Creating the reports...";
                    break;
                default:
                    message = "";
                    break;
            }
            return message;
        }

        private string GenerateStatusMessage(TimeSpan duration)
        {
            return $"Processing took {duration.TotalSeconds.ToString("F3")} seconds.";
        }
    }
}
