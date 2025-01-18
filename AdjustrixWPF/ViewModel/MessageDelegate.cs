using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustrixWPF.ViewModel
{
    public class MessageDelegate
    {
        private string message = "";

        public event Action<string> MessageChanged;

        public void ChangeMessage(string message)
        {
            this.message = message;
            MessageChanged?.Invoke(this.message);
        }
    }
}
