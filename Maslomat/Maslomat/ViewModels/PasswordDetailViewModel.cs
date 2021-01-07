using Maslomat.Views;
using PropertyChanged;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Maslomat.ViewModels
{
    [QueryProperty(nameof(ItemID), nameof(ItemID))]
    class PasswordDetailViewModel : BaseViewModel
    {
        string _itemID;
        [DoNotNotify]
        public string ItemID
        {
            get => _itemID;
            set
            {
                _itemID = value;
                _passwordID = int.Parse(value);
                LoadItem(_passwordID);
            }
        }
        int _passwordID;

        public string Designation { get; set; }
        public string Password { get; set; }

        public bool HidePassword { get; set; } = true;


        public Command EditCommand { get; }
        public Command TogglePasswordCommand { get; }
        public Command CopyPasswordCommand { get; }
        public Command DeleteCommand { get; }

        public PasswordDetailViewModel()
        {
            EditCommand = new Command(async () => await SwitchToEdit());
            TogglePasswordCommand = new Command(() => HidePassword = !HidePassword);
            CopyPasswordCommand = new Command(CopyPassword);
            DeleteCommand = new Command(DeletePassword);
        }

        private async void DeletePassword()
        {
            if (await Shell.Current.DisplayAlert("Delete Password", $"Are you sure you want to remove {Designation}?", "Yes", "No"))
            {
                await PasswordDatabase.DeleteItemAsync(_passwordID);
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void LoadItem(int id)
        {
            var item = await PasswordDatabase.GetItemAsync(id);
            Designation = item.Designation;
            Password = item.Password;
        }

        private async Task SwitchToEdit()
        {
            await Shell.Current.GoToAsync($"{nameof(PasswordEditPage)}?{nameof(PasswordEditViewModel.ItemID)}={ItemID}");
        }

        private async void CopyPassword()
        {
            await Shell.Current.DisplayAlert("", "Password copied.", "OK");
            await Clipboard.SetTextAsync(Password);
        }
    }
}
