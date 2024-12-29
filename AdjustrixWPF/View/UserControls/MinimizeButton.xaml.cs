using System;
using System.Windows;
using System.Windows.Controls;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for MinimizeButton.xaml
    /// </summary>
    public partial class MinimizeButton : UserControl
    {
        public MinimizeButton()
        {
            InitializeComponent();
        }

        public event EventHandler Click;

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
