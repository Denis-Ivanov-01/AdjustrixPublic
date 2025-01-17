using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adjustment.Project;
using Adjustment;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AdjustrixWPF.View.UserControls;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectDataViewModel : ViewModelBase
    {
        private ProjectContainer projectContainer;
        private AdjustrixProject currentProject;
        private ProjectType projectType;
        private ObservableCollection<HeightDelta> levelingMeasurements;
        private ObservableCollection<KnownBenchmark> knownBenchmarks;
        private LanguageViewModel languageViewModel;

        public ICommand ToggleMeasurement { get; }

        public ICommand EditMeasurement { get; }

        public ICommand EditPoint { get; }

        public ObservableCollection<HeightDelta> LevelingMeasurements
        {
            get
            {
                return levelingMeasurements;
            }
            set
            {
                levelingMeasurements = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<KnownBenchmark> KnownBenchmarks
        {
            get
            {
                return knownBenchmarks;
            }
            set
            {
                knownBenchmarks = value;
                OnPropertyChanged();
            }
        }

        public LanguageViewModel LanguageViewModel
        {
            get
            {
                return languageViewModel;
            }
        }
        public ProjectDataViewModel(ProjectContainer container)
        {
            languageViewModel = LanguageViewModel.Singleton;
            projectContainer = container;
            levelingMeasurements = new ObservableCollection<HeightDelta>();
            knownBenchmarks= new ObservableCollection<KnownBenchmark>();
            projectContainer.ProjectChanged += OnProjectChanged;
            projectContainer.ProjectTypeChanged += OnProjectTypeChanged;

            ToggleMeasurement = new RelayCommand(ToggleMeasurementEnabled, CanToggleMeasurement);
            EditMeasurement = new RelayCommand(EditMeasurementValue);
            EditPoint = new RelayCommand(EditPointValue);
        }

        private void EditMeasurementValue(object parameter)
        {
            HeightDelta delta = (HeightDelta)parameter;
            int deltaIndex = LevelingMeasurements.IndexOf(delta);
            EditMeasurementPrompt prompt = new(delta);
            prompt.ShowDialog();
            if (prompt.Edit)
            {
                LevelingMeasurements[deltaIndex].Value = prompt.Value;
                PublishProjectChanges();
            }
        }

        public void EditPointValue(object parameter)
        {
            KnownBenchmark point = (KnownBenchmark)parameter;
            int pointIndex = KnownBenchmarks.IndexOf(point);
            EditPointPrompt prompt = new(point);
            prompt.ShowDialog();
            if (prompt.Edit)
            {
                KnownBenchmarks[pointIndex].Value = prompt.Value;
                UpdatePointsInMeasurements(point);
                PublishProjectChanges();
            }
        }

        private void ToggleMeasurementEnabled(object parameter)
        {
            HeightDelta delta = (HeightDelta)parameter;
            delta.IsEnabled = delta.IsEnabled? false: true;
            PublishProjectChanges();
        }

        private bool CanToggleMeasurement(object parameter)
        {
            return true;
        }


        private void OnProjectTypeChanged(ProjectType type)
        {
            projectType = type;
        }

        private void OnProjectChanged(AdjustrixProject project)
        {
            currentProject = project;
            switch (projectType)
            {
                case ProjectType.Leveling:
                    LevelingProject currProject = (LevelingProject)currentProject;
                    SwitchLevelingMeasurements(currProject);
                    SwitchKnownBenchmarks(currProject);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SwitchLevelingMeasurements(LevelingProject project)
        {
            LevelingMeasurements.Clear();
            if (project.HeightDifferences != null)
            {
                foreach(HeightDelta delta in project.HeightDifferences)
                {
                    LevelingMeasurements.Add(delta);
                }
                //OnPropertyChanged(nameof(LevelingMeasurements));
            }
        }

        private void SwitchKnownBenchmarks(LevelingProject project)
        {
            KnownBenchmarks.Clear();
            if (project.KnownBenchmarks != null)
            {
                foreach(KnownBenchmark benchmark in project.KnownBenchmarks)
                {
                    KnownBenchmarks.Add(benchmark);
                }
            }
        }

        private void PublishProjectChanges()
        {
            UpdateProject();
            projectContainer.ChangeProject(currentProject);
        }

        private void UpdateProject()
        {
            switch (projectType)
            {
                case ProjectType.Leveling:
                    LevelingProject project = (LevelingProject)currentProject;
                    project.HeightDifferences = LevelingMeasurements.ToList();
                    project.KnownBenchmarks = KnownBenchmarks.ToHashSet();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void UpdatePointsInMeasurements(KnownBenchmark newPoint)
        {
            foreach (HeightDelta meas in LevelingMeasurements)
            {
                if (meas.FromPoint.Number == newPoint.Number)
                {
                    meas.FromPoint = newPoint;
                }
                else if (meas.ToPoint.Number == newPoint.Number)
                {
                    meas.ToPoint = newPoint;
                }
            }
        }
    }
}
