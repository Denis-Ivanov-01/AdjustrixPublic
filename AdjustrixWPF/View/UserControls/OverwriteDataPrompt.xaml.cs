using System.Windows;
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
