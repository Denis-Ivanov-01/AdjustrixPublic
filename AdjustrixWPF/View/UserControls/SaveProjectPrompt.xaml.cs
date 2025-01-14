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
    /// Interaction logic for SaveProjectPrompt.xaml
    /// </summary>
    public partial class SaveProjectPrompt : Window
    {
        private bool saveProject = false;

        public bool SaveProject 
        {
            get { return saveProject; }
        }

        public SaveProjectPrompt(ProjectViewModel projectViewModel)
        {
            this.DataContext = projectViewModel;
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            saveProject = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            saveProject= false;
            Close();
        }
    }
}
