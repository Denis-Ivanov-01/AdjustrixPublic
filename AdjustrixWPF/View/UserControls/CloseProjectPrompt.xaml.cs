using System.Windows;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for CloseProjectPrompt.xaml
    /// </summary>
    public partial class CloseProjectPrompt : Window
    {

        private bool closeProject;

        public bool CloseProject
        {
            get { return closeProject; }
            set { closeProject = value; }
        }


        public CloseProjectPrompt(ProjectViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            CloseProject = true;
            Close();
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            CloseProject = false;
            Close();
        }
    }
}
