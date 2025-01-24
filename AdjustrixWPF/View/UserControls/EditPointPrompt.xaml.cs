using System.Windows;
using System.Windows.Controls;
using Adjustment;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for EditPointPrompt.xaml
    /// </summary>
    public partial class EditPointPrompt : Window
    {
        private readonly AdjustrixViewModel viewModel;

        private bool valueIsValid = false;

        public double Value { get; set; }

        public bool Edit { get; set; }

        public EditPointPrompt(KnownBenchmark point, AdjustrixViewModel viewModel)
        {
            this.DataContext = viewModel;
            this.viewModel = viewModel;
            InitializeComponent();
            InputBox.Text = point.Value.ToString();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(InputBox.Text, out double value))
            {
                if (value < 0)
                {
                    MessageBox.Text = viewModel.LanguageViewModel.NegativeValueMessage;
                    valueIsValid = false;
                    return;
                }

                Value = value;
                MessageBox.Text = "";
                valueIsValid = true;
            }
            else
            {
                valueIsValid = false;
                MessageBox.Text = viewModel.LanguageViewModel.NonNumericMessage;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (valueIsValid)
            {
                Edit = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Edit = false;
            Close();
        }
    }
}
