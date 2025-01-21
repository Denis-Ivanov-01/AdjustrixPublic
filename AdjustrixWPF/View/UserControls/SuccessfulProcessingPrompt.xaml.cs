using System.Windows;

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
