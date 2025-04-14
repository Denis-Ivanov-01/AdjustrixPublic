using System.Windows;
using System.Windows.Input;

namespace AdjustrixWPF.View.UserControls.Composite
{
    /// <summary>
    /// Interaction logic for PythonScriptsWindow.xaml
    /// </summary>
    public partial class PythonScriptsWindow : Window
    {
        public PythonScriptsWindow()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
