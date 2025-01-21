using System.Windows;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for ProcessingErrorPrompt.xaml
    /// </summary>
    public partial class ProcessingErrorPrompt : Window
    {
        public ProcessingErrorPrompt(string errorMessage)
        {
            InitializeComponent();
            ErrorMessageTextBlock.Text = errorMessage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
