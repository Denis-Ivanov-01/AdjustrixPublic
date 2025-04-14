using System.Windows;
using System.Windows.Controls;

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
