using System.Windows;
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
