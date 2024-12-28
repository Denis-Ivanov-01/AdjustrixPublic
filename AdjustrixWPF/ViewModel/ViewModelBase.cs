using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AdjustrixWPF.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string member = null)
        {
            PropertyChanged?.Invoke(member, new PropertyChangedEventArgs(member));
        }
    }
}
