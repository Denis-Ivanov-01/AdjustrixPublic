using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adjustment.Project;
using Adjustment;
using System.Collections.ObjectModel;

namespace AdjustrixWPF.ViewModel
{
    public class DataViewModel : ViewModelBase
    {
        private ProjectStore store;
        private AdjustrixProject currentProject;
        private ProjectType projectType;
        private ObservableCollection<HeightDelta> levelingMeasurements;

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
        public DataViewModel(ProjectStore store)
        {
            this.store = store;
            levelingMeasurements = new ObservableCollection<HeightDelta>();
            store.ProjectChanged += OnProjectChanged;
            store.ProjectTypeChanged += OnProjectTypeChanged;
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
                    SwitchLevelingMeasurements((LevelingProject)currentProject);
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
    }
}
