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
    /// Interaction logic for OverwriteDataPrompt.xaml
    /// </summary>
    public partial class OverwriteDataPrompt : Window
    {

        public bool OverwriteData { get; private set; }

        public OverwriteDataPrompt(AdjustrixViewModel viewModel)
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            OverwriteData = false;
            Close();
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            OverwriteData = true;
            Close();
        }
    }
}
