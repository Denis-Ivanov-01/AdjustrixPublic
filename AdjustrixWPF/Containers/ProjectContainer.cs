using System;
using AdjustrixBase.Project;

namespace AdjustrixWPF.Containers
{
    public class ProjectContainer
    {

        private AdjustrixProject project;

        public AdjustrixProject Project
        {
            get { return project; }
            private set { project = value; }
        }

        private string projectFolder;

        public string ProjectFolder
        {
            get
            {
                return projectFolder;
            }
            private set
            {
                projectFolder = value;
            }
        }

        public event Action<AdjustrixProject> ProjectChanged;
        public event Action<ProjectType> ProjectTypeChanged;
        public event Action<bool> ProjectChangesChanged;
        public event Action<string> ProjectFolderChanged;


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

        public void ChangeProjectFolder(string folder)
        {
            ProjectFolder = folder;
            ProjectFolderChanged?.Invoke(ProjectFolder);
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
