using System.Windows;
using AdjustrixWPF.ViewModel;

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

        public SuccessfulProcessingPrompt(ViewModelBase viewModel)
        {
            this.DataContext = viewModel;
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
