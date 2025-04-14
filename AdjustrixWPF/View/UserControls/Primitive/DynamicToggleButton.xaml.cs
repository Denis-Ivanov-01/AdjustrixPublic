using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdjustrixWPF.View.UserControls.Primitive
{

    /// <summary>
    /// Interaction logic for DynamicToggleButton.xaml
    /// </summary>
    public partial class DynamicToggleButton : UserControl
    {


        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                nameof(IsEnabled),
                typeof(bool),
                typeof(DynamicToggleButton),
                new PropertyMetadata(default(bool)));

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(DynamicToggleButton),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(DynamicToggleButton),
                new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        // Dependency Property for ToolTip
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register(
                nameof(ToolTip),
                typeof(string),
                typeof(DynamicToggleButton),
                new PropertyMetadata(string.Empty));

        public string ToolTip
        {
            get => (string)GetValue(ToolTipProperty);
            set => SetValue(ToolTipProperty, value);
        }

        public DynamicToggleButton()
        {
            InitializeComponent();
        }
    }
}
