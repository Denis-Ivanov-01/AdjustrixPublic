using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectViewModel : ViewModelBase
    {
        private bool projectLoaded=false;
        private float saveProjectOpacity = 0.0f;

        public ProjectViewModel()
        {

        }


        public bool ProjectLoaded
        {
            get { return projectLoaded; }
            set { projectLoaded = value; }
        }
    }
}
