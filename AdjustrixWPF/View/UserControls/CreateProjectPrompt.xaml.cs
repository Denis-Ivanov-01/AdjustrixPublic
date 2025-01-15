using System.Windows;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for ManageProjectPrompt.xaml
    /// </summary>
    public partial class CreateProjectPrompt : Window
    {
        public bool Confirmed { get; set; }
        public CreateProjectPrompt(ProjectViewModel projectViewModel)
        {
            this.DataContext = projectViewModel;
            InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }
    }
}
