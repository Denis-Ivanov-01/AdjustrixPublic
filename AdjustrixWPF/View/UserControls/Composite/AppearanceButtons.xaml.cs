using System.Windows;
using System.Windows.Controls;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls.Composite
{
    /// <summary>
    /// Interaction logic for AppearanceButtons.xaml
    /// </summary>
    public partial class AppearanceButtons : UserControl
    {
        public AppearanceButtons()
        {
            InitializeComponent();
        }



        public ViewModelBase viewModelBase
        {
            get { return (ViewModelBase)GetValue(viewModelBaseProperty); }
            set
            {
                SetValue(viewModelBaseProperty, value);
                this.DataContext = value;
            }
        }

        // Using a DependencyProperty as the backing store for viewModelBase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty viewModelBaseProperty =
            DependencyProperty.Register("viewModelBase", typeof(ViewModelBase), typeof(AppearanceButtons), new PropertyMetadata(new MainWindowViewModel()));
    }
}
