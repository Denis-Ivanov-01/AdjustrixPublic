using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
