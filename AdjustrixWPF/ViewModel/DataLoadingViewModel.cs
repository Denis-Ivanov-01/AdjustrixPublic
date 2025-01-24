using System;
using win=System.Windows;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Adjustment;
using Adjustment.NetworkAnalysis;
using Adjustment.Project;
using AdjustrixWPF.Model;
using AdjustrixWPF.View.UserControls;
using MaterialDesignThemes.Wpf;
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

        private async void ReadExcelFile(object param)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "Microsoft Excel file (.xlsx / .xls)|*.xlsx;*.xls";
            dialog.DefaultExt = ".xlsx/.xls";
            Forms.DialogResult result = dialog.ShowDialog();
            if (result == Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {//todo: add data validation here. Now it is done when the adjustment is started
                await Task.Run(() => TryReadExcel(dialog.FileName));
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
                win.Application.Current.Dispatcher.Invoke(() =>
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
                    win.Application.Current.Dispatcher.Invoke(() =>
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
    }
}
