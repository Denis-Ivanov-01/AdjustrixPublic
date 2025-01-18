using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustrixWPF.ViewModel
{
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
