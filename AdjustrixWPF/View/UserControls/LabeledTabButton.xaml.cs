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

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for TabButton.xaml
    /// </summary>
    public partial class LabeledTabButton : UserControl
    {
        public LabeledTabButton()
        {
            InitializeComponent();
        }

        public int Height
        {
            get { return (int)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(int), typeof(LabeledTabButton), new PropertyMetadata(20));



        public int Width
        {
            get { return (int)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(int), typeof(LabeledTabButton), new PropertyMetadata(20));



        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(LabeledTabButton), new PropertyMetadata("Placeholder"));


        public int ContentSize
        {
            get { return (int)GetValue(ContentSizeProperty); }
            set { SetValue(ContentSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentSizeProperty =
            DependencyProperty.Register("ContentSize", typeof(int), typeof(LabeledTabButton), new PropertyMetadata(17));



        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(LabeledTabButton), new PropertyMetadata("Label"));
    }
}
