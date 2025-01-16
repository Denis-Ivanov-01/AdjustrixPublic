using System.Windows;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for SaveProjectPrompt.xaml
    /// </summary>
    public partial class SaveProjectPrompt : Window
    {
        private bool saveProject = false;

        public bool SaveProject
        {
            get { return saveProject; }
        }

        public SaveProjectPrompt(ProjectFileViewModel projectViewModel)
        {
            this.DataContext = projectViewModel;
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            saveProject = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            saveProject = false;
            Close();
        }
    }
}
