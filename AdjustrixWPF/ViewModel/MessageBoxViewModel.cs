using AdjustrixWPF.Containers;

namespace AdjustrixWPF.ViewModel
{
    /// <summary>
    /// Handling the message box at the bottom of the window.
    /// It displays messages like "Project opened", "Data loaded" and other mundane things
    /// </summary>
    public class MessageBoxViewModel : ViewModelBase
    {
        private string message = "";

        public string Message
        {
            get { return message; }
            private set
            {
                message = value;
                OnPropertyChanged();
            }
        }

        public MessageBoxViewModel(MessageDelegate container)
        {
            container.MessageChanged += OnMessageChanged;
        }

        private void OnMessageChanged(string message)
        {
            Message = message;
        }
    }
}
