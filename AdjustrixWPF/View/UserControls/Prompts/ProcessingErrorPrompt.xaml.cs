using System.Windows;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for ProcessingErrorPrompt.xaml
    /// </summary>
    public partial class ProcessingErrorPrompt : Window
    {
        public ProcessingErrorPrompt(AdjustrixViewModel viewModel, string errorMessage)
        {
            this.DataContext = viewModel;
            InitializeComponent();
            ErrorMessageTextBlock.Text = errorMessage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
