using System;
using Adjustment.Project;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectContainer
    {
        private AdjustrixProject project;

        public AdjustrixProject Project
        {
            get { return project; }
            private set { project = value; }
        }

        public event Action<AdjustrixProject> ProjectChanged;
        public event Action<ProjectType> ProjectTypeChanged;
        public event Action<bool> ProjectChangesChanged;

        
        public void ChangeProject(AdjustrixProject project)
        {
            Project = project;
            HasUnsavedChanges = true;
            ProjectChanged?.Invoke(Project);
            ProjectChangesChanged?.Invoke(true);
        }

        public void ChangeProjectType(ProjectType type)
        {
            ProjectTypeChanged?.Invoke(type);
        }

        public void SetChanges(bool value)
        {
            HasUnsavedChanges = value;
            ProjectChangesChanged?.Invoke(value);
        }

        private bool hasUnsavedChanges;

        public bool HasUnsavedChanges
        {
            get { return hasUnsavedChanges; }
            private set
            {
                if (hasUnsavedChanges != value)
                {
                    hasUnsavedChanges = value;
                    //ProjectChanged?.Invoke(this);
                }
            }
        }

    }
}
