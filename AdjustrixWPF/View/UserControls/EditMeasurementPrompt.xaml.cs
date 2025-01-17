using System.Windows;
using System.Windows.Controls;
using Adjustment;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for EditMeasurementPrompt.xaml
    /// </summary>
    public partial class EditMeasurementPrompt : Window
    {
        private bool valueIsValid = false;

        public double Value { get; set; }

        public bool Edit { get; set; }

        public EditMeasurementPrompt(HeightDelta measurement)
        {
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
                MessageBox.Text = "Cannot enter non-numeric value!";
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
