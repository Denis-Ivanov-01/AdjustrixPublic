using System;
using Adjustment.Project;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectStore
    {
        public event Action<AdjustrixProject> ProjectChanged;
        public event Action<ProjectType> ProjectTypeChanged;
        public void ChangeProject(AdjustrixProject project)
        {
            ProjectChanged?.Invoke(project);
        }

        public void ChangeProjectType(ProjectType type)
        {
            ProjectTypeChanged?.Invoke(type);
        }
    }
}
