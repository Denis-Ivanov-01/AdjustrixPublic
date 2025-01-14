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
    public partial class ManageProjectPrompt : Window
    {
        public ManageProjectPrompt(ProjectViewModel projectViewModel)
        {
            this.DataContext = projectViewModel;
            InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
