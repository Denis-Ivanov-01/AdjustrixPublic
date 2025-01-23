using System;
using System.Windows.Forms;
using System.Windows.Input;
using Adjustment.Project;
using AdjustrixWPF.Model;
using AdjustrixWPF.View.UserControls;
using Forms = System.Windows.Forms;

namespace AdjustrixWPF.ViewModel
{
    public class DataLoadingViewModel : AdjustrixViewModel
    {
        private ProjectContainer projectContainer;
        private MessageDelegate messageDelegate;

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

        private void ReadExcelFile(object param)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "Microsoft Excel file (.xlsx / .xls)|*.xlsx;*.xls";
            dialog.DefaultExt = ".xlsx/.xls";
            Forms.DialogResult result = dialog.ShowDialog();
            if (result == Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {//todo: add data validation here. Now it is done when the adjustment is started
                try
                {
                    ReadExcelForType(dialog.FileName);
                }
                catch (Exception ex)
                {
                    ProcessingErrorPrompt prompt = new(this, ex.Message);
                    prompt.ShowDialog();
                }
            }

        }

        private void ReadExcelForType(string excelPath)
        {
            switch (currentProjectType)
            {
                case ProjectType.Leveling:
                    LevelingExcelReader reader = new(excelPath);
                    LevelingProject project = (LevelingProject)currentProject;
                    project.KnownBenchmarks = reader.KnownBenchmarks;
                    project.HeightDifferences = reader.Measurements;
                    projectContainer.ChangeProject(project);
                    messageDelegate.ChangeMessage($"Loaded {reader.KnownBenchmarks.Count} benchmarks and {reader.Measurements.Count} measurements.");
                    break;
                default:
                    throw new NotImplementedException("No logic for this project type is supported!");
            }
        }


        public bool CanReadExcel(object param)
        {
            return currentProject != null;
        }
    }
}
