using Maslomat.Models;
using System.ComponentModel;
using Xamarin.Forms;

namespace Maslomat.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public bool SystemBusy { get; set; }

        public PasswordDatabase PasswordDatabase = DependencyService.Get<PasswordDatabase>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
