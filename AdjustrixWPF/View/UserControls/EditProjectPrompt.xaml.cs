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

namespace AdjustrixWPF.View
{
    /// <summary>
    /// Interaction logic for EditProjectPrompt.xaml
    /// </summary>
    public partial class EditProjectPrompt : Window
    {
        public bool ApplyEdits
        {
            get; private set;
        }

        public EditProjectPrompt(ProjectFileViewModel viewModel)
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            ApplyEdits = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            ApplyEdits = false;
            Close();
        }
    }
}
