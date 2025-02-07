using System.Windows;
using AdjustrixWPF.Enums;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls
{

    /// <summary>
    /// Interaction logic for YesNoPrompt.xaml
    /// </summary>
    public partial class YesNoPrompt : Window
    {

        public YesNoPromptResult Result { get; set; }

        public YesNoPrompt(AdjustrixViewModel viewModel, string promptText)
        {
            this.DataContext = viewModel;
            InitializeComponent();
            PromptTextBlock.Text = promptText;
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            Result = YesNoPromptResult.Yes;
            Close();
        }
        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            Result = YesNoPromptResult.No;
            Close();
        }
    }
}
