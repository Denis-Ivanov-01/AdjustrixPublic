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

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for SuccessfulProcessingPrompt.xaml
    /// </summary>
    public partial class SuccessfulProcessingPrompt : Window
    {
        public bool RevealReports
        {
            get; private set;
        }

        public SuccessfulProcessingPrompt()
        {
            InitializeComponent();
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            RevealReports = true;
            Close();
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            RevealReports = false;
            Close();

        }
    }
}
