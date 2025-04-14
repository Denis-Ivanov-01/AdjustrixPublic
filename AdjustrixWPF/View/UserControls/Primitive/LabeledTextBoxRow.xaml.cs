using System.Windows;
using System.Windows.Controls;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for LabeledTextBoxRow.xaml
    /// </summary>
    public partial class LabeledTextBoxRow : UserControl
    {

        public int ControlWidth
        {
            get { return (int)GetValue(ControlWidthProperty); }
            set { SetValue(ControlWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlWidthProperty =
            DependencyProperty.Register("ControlWidth", typeof(int), typeof(LabeledTextBoxRow), new PropertyMetadata(230));



        public string TextBoxText
        {
            get { return (string)GetValue(TextBoxTextProperty); }
            set { SetValue(TextBoxTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxTextProperty =
            DependencyProperty.Register("TextBoxText", typeof(string), typeof(LabeledTextBoxRow), new PropertyMetadata(""));



        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledTextBoxRow), new PropertyMetadata(""));

        public LabeledTextBoxRow()
        {
            InitializeComponent();
        }
    }
}
