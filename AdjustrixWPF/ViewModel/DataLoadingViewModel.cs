using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Win = System.Windows;
using AdjustrixBase.Project;
using AdjustrixBase.DataModels;
using AdjustrixBase.NetworkAnalysis;
using AdjustrixWPF.View.UserControls;
using AdjustrixWPF.Containers;
using AdjustrixWPF.DataLoading;
using AdjustrixWPF.Commands;

namespace AdjustrixWPF.ViewModel
{
    /// <summary>
    /// Intended for the default data loading capabilities.
    /// Custom data loading can be added by using Python scripts.
    /// </summary>
    public class DataLoadingViewModel : AdjustrixViewModel
    {
        private readonly ProjectContainer projectContainer;
        private readonly MessageDelegate messageDelegate;

        private AdjustrixProject currentProject;
        private ProjectType currentProjectType;

        public ICommand ReadExcel { get; }

        public DataLoadingViewModel(ProjectContainer projectContainer, MessageDelegate messageDelegate)
        {
            this.projectContainer = projectContainer;
            this.messageDelegate = messageDelegate;
            this.projectContainer.ProjectChanged += OnProjectChanged;
            this.projectContainer.ProjectTypeChanged += OnProjectTypeChanged;

            ReadExcel = new RelayCommand(ReadExcelFile, CanReadExcel);
        }

        private void OnProjectTypeChanged(ProjectType projType)
        {
            currentProjectType = projType;
        }

        private void OnProjectChanged(AdjustrixProject proj)
        {
            currentProject = proj;
        }

        private async void ReadExcelFile(object param)
        {
            if (HandleExistingData())
            {
                OpenFileDialog dialog = new();
                dialog.Filter = "Microsoft Excel file (.xlsx / .xls)|*.xlsx;*.xls";
                dialog.DefaultExt = ".xlsx/.xls";
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    await Task.Run(() => TryReadExcel(dialog.FileName));
                }
            }

        }

        private void TryReadExcel(string filePath)
        {
            try
            {
                ReadExcelForType(filePath);
            }
            catch (Exception ex)
            {
                Win.Application.Current.Dispatcher.Invoke(() =>
                {
                    string message = ErrorMessageGenerator.GenerateMessage(ex);
                    ProcessingErrorPrompt prompt = new(this, message);
                    prompt.ShowDialog();
                });
            }
        }

        private void ReadExcelForType(string excelPath)
        {
            switch (currentProjectType)
            {
                case ProjectType.Leveling:
                    LevelingExcelReader reader = new(excelPath);

                    //In the Graph constructor, there are validations.
                    Graph<HeightDelta> graph = new(reader.Measurements);

                    LevelingProject project = (LevelingProject)currentProject;
                    project.KnownBenchmarks = reader.KnownBenchmarks;
                    project.HeightDifferences = reader.Measurements;
                    Win.Application.Current.Dispatcher.Invoke(() =>
                    {
                        projectContainer.ChangeProject(project);
                        string message = string.Format(LanguageViewModel.DataLoadedMessagePattern, reader.Measurements.Count, reader.KnownBenchmarks.Count);
                        messageDelegate.ChangeMessage(message);
                    });
                    break;
                default:
                    throw new NotImplementedException("No logic for this project type is supported!");
            }
        }

        public bool CanReadExcel(object param)
        {
            return currentProject != null;
        }

        private bool HandleExistingData()
        {
            switch (currentProjectType)
            {
                case ProjectType.Leveling:
                    LevelingProject levelingProject = (LevelingProject)currentProject;
                    if (levelingProject.KnownBenchmarks.Count > 0 || levelingProject.HeightDifferences.Count > 0)
                    {
                        return PromptOverwrite();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        private bool PromptOverwrite()
        {
            YesNoPrompt prompt = new(this, LanguageViewModel.OverwriteDataQuestion);
            prompt.ShowDialog();
            return prompt.Result == Enums.YesNoPromptResult.Yes;
        }
    }
}
