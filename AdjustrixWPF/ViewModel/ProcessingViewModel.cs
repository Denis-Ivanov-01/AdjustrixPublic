using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AdjustrixBase.Adjustment.Leveling;
using AdjustrixBase.DataModels;
using AdjustrixBase.Extensions;
using AdjustrixBase.NetworkAnalysis;
using AdjustrixBase.Project;
using AdjustrixBase.Adjustment.Base;
using AdjustrixWPF.Containers;
using AdjustrixWPF.Commands;
using AdjustrixWPF.View.UserControls;

namespace AdjustrixWPF.ViewModel
{
    public class ProcessingViewModel : AdjustrixViewModel
    {
        private readonly ProjectContainer projectContainer;
        private AdjustrixProject currentProject;
        private string projectFolder;

        private AdjustmentStatus status;
        private readonly AdjustmentStatusDelegate statusDelegate;

        private readonly Graph<HeightDelta> graph;

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
                ProcessingErrorPrompt prompt = new(this, LanguageViewModel.ProjectFolderNotFoundMessage);
                prompt.ShowDialog();
                return;
            }
            await Task.Run(() =>
            {
                TryPerformProcessing(projectFolder);
            });
        }

        private void TryPerformProcessing(string projectFolder)
        {
            try
            {
                LevelingProject project = (LevelingProject)currentProject;
                AdjustrixBase.Adjustment.Base.Language lang = GetAdjustmentLanguage();
                LevelingProcessing processing = new(project, statusDelegate, lang);
                processing.Process(projectFolder);
            }
            catch (Exception ex)
            {
                // Show the error prompt on the UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string message = ErrorMessageGenerator.GenerateMessage(ex);
                    ProcessingErrorPrompt p = new(this, message);
                    p.ShowDialog();
                });
                return;
            }

            // Show the success prompt on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                SuccessfulProcessingPrompt prompt = new(this);
                prompt.ShowDialog();
                if (prompt.RevealReports)
                {
                    Process.Start("explorer.exe", projectFolder);
                }
            });
        }

        private static AdjustrixBase.Adjustment.Base.Language GetAdjustmentLanguage()
        {
            return LanguageViewModel.SelectedLanguage == Enums.Language.Bulgarian ? 
                AdjustrixBase.Adjustment.Base.Language.Bulgarian :
                AdjustrixBase.Adjustment.Base.Language.English;
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
                    message = LanguageViewModel.PreparingDataMessage;
                    break;
                case AdjustmentStatus.AnalyzingNetwork:
                    message = LanguageViewModel.PerformingGeometricAnalysis;
                    break;
                case AdjustmentStatus.CalculatingAdjustment:
                    message = LanguageViewModel.PerformingAdjustmentMessage;
                    break;
                case AdjustmentStatus.CreatingReports:
                    message = LanguageViewModel.CreatingReportsMessage;
                    break;
                default:
                    message = "";
                    break;
            }
            return message;
        }

        private string GenerateStatusMessage(TimeSpan duration)
        {
            return string.Format(LanguageViewModel.ProcessingTimeMessage, duration.TotalSeconds.ToString("F3"));
        }
    }
}
