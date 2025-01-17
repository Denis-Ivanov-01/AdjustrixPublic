using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Adjustment.Project;

namespace AdjustrixWPF.ViewModel
{
    public class ProcessingViewModel : ViewModelBase
    {
        private ProjectContainer projectContainer;
        private AdjustrixProject currentProject;
        public ICommand Adjust { get; }

        public ProcessingViewModel(ProjectContainer projectContainer) 
        {
            this.projectContainer = projectContainer;
            projectContainer.ProjectChanged += OnProjectChanged;
        }

        private void OnProjectChanged(AdjustrixProject project)
        {
            currentProject = project;
        }
    }
}
