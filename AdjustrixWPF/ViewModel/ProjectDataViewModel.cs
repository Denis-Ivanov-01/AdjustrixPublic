using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adjustment.Project;
using Adjustment;
using System.Collections.ObjectModel;
using System.Windows.Input;

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

        public ICommand DeleteMeasurement { get; }

        public ICommand EditMeasurement { get; }

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

            DeleteMeasurement = new RelayCommand(RemoveMeasurement, CanRemoveMeasurement);
        }

        private void RemoveMeasurement(object parameter)
        {
            HeightDelta delta = (HeightDelta)parameter;
            LevelingMeasurements.Remove(delta);
            UpdateProject();
            projectContainer.ChangeProject(currentProject);
        }

        private bool CanRemoveMeasurement(object parameter)
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
    }
}
