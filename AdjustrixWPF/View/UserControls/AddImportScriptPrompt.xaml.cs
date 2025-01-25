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
using AdjustrixWPF.Model;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for AddImportScriptPrompt.xaml
    /// </summary>
    public partial class AddImportScriptPrompt : Window
    {

        public bool RegisterScript { get; set; }
        public AddImportScriptPrompt(ImportScriptsViewModel viewModel)
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            RegisterScript = true;
            Close();
        }
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            RegisterScript = false;
            Close();
        }
    }
}
