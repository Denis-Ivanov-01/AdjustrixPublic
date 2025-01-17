using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Adjustment;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for EditPointPrompt.xaml
    /// </summary>
    public partial class EditPointPrompt : Window
    {
        private bool valueIsValid = false;

        public double Value { get; set; }

        public bool Edit { get; set; }

        public EditPointPrompt(KnownBenchmark point)
        {
            InitializeComponent();
            InputBox.Text = point.Value.ToString();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(InputBox.Text, out double value))
            {
                if (value < 0)
                {
                    MessageBox.Text = "Cannot enter negative value!";
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
