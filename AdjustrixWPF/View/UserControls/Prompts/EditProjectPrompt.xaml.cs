using System.Windows;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.View
{
    /// <summary>
    /// Interaction logic for EditProjectPrompt.xaml
    /// </summary>
    public partial class EditProjectPrompt : Window
    {
        public bool ApplyEdits
        {
            get; private set;
        }

        public EditProjectPrompt(ProjectFileViewModel viewModel)
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            ApplyEdits = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            ApplyEdits = false;
            Close();
        }
    }
}
