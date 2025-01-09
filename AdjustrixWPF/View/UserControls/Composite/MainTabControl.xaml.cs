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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdjustrixWPF.View.UserControls.Composite
{
    /// <summary>
    /// Interaction logic for MainTabControl.xaml
    /// </summary>
    public partial class MainTabControl : UserControl
    {
        public MainTabControl()
        {
            InitializeComponent();
        }



        public int Width
        {
            get { return (int)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(int), typeof(MainTabControl), new PropertyMetadata(200));

        private void TabHeader_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
