using System.Windows;
using System.Windows.Controls;
using AdjustrixBase.DataModels;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for EditMeasurementPrompt.xaml
    /// </summary>
    public partial class EditMeasurementPrompt : Window
    {
        private readonly AdjustrixViewModel viewModel;

        private bool valueIsValid = false;

        public double Value { get; set; }

        public bool Edit { get; set; }

        public EditMeasurementPrompt(HeightDelta measurement, AdjustrixViewModel viewModel)
        {
            this.DataContext = viewModel;
            this.viewModel = viewModel;
            InitializeComponent();
            InputBox.Text = measurement.Value.ToString();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(InputBox.Text, out double value))
            {
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

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
