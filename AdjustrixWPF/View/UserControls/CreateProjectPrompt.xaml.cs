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
    /// Interaction logic for ManageProjectPrompt.xaml
    /// </summary>
    public partial class CreateProjectPrompt : Window
    {
        public bool Confirmed { get; set; }
        public CreateProjectPrompt(ProjectViewModel projectViewModel)
        {
            this.DataContext = projectViewModel;
            InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }
    }
}
