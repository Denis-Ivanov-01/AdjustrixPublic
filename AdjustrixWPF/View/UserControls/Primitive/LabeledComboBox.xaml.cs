using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for LabeledComboBox.xaml
    /// </summary>
    public partial class LabeledComboBox : UserControl
    {
        public LabeledComboBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledComboBox), new PropertyMetadata("Label"));

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        // Dependency Property for ComboBox ItemsSource
        public static readonly DependencyProperty ComboBoxItemsSourceProperty =
            DependencyProperty.Register("ComboBoxItemsSource", typeof(IEnumerable), typeof(LabeledComboBox));

        public IEnumerable ComboBoxItemsSource
        {
            get => (IEnumerable)GetValue(ComboBoxItemsSourceProperty);
            set => SetValue(ComboBoxItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ComboBoxSelectedItemProperty =
        DependencyProperty.Register(nameof(ComboBoxSelectedItem), typeof(string), typeof(LabeledComboBox));

        public object ComboBoxSelectedItem
        {
            get => GetValue(ComboBoxSelectedItemProperty);
            set => SetValue(ComboBoxSelectedItemProperty, value);
        }

        // Dependency Property for BorderRow
        public static readonly DependencyProperty BorderRowProperty =
            DependencyProperty.Register("BorderRow", typeof(int), typeof(LabeledComboBox), new PropertyMetadata(1));

        public int BorderRow
        {
            get => (int)GetValue(BorderRowProperty);
            set => SetValue(BorderRowProperty, value);
        }

        // Dependency Property for BorderRowSpan
        public static readonly DependencyProperty BorderRowSpanProperty =
            DependencyProperty.Register("BorderRowSpan", typeof(int), typeof(LabeledComboBox), new PropertyMetadata(2));

        public int BorderRowSpan
        {
            get => (int)GetValue(BorderRowSpanProperty);
            set => SetValue(BorderRowSpanProperty, value);
        }

        // Dependency Property for ComboBoxRow
        public static readonly DependencyProperty ComboBoxRowProperty =
            DependencyProperty.Register("ComboBoxRow", typeof(int), typeof(LabeledComboBox), new PropertyMetadata(2));

        public int ComboBoxRow
        {
            get => (int)GetValue(ComboBoxRowProperty);
            set => SetValue(ComboBoxRowProperty, value);
        }

        // Dependency Property for LabelRow
        public static readonly DependencyProperty LabelRowProperty =
            DependencyProperty.Register("LabelRow", typeof(int), typeof(LabeledComboBox), new PropertyMetadata(1));

        public int LabelRow
        {
            get => (int)GetValue(LabelRowProperty);
            set => SetValue(LabelRowProperty, value);
        }
    }
}
