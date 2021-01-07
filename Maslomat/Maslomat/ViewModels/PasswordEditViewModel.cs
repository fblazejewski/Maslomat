using PropertyChanged;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Maslomat.ViewModels
{
    [QueryProperty(nameof(ItemID), nameof(ItemID))]
    class PasswordEditViewModel : BaseViewModel
    {
        public string Title { get; set; } = "Add Password";
        public string Message { get; set; }

        int _id { get; set; }
        public string Designation { get; set; }
        public string Password { get; set; }
        public bool HiddingPassword { get; set; } = true;

        [DoNotNotify]
        public string ItemID
        {
            get => _id.ToString();
            set
            {
                _id = int.Parse(value);
                LoadItemFromID(_id);
            }
        }


        public Command SaveItemCommand { get; }
        public Command TogglePasswordVievCommand { get; }
        public Command CopyPasswordCommand { get; }


        public bool NewPassword { get; set; } = true;

        public PasswordEditViewModel()
        {
            SaveItemCommand = new Command(async () => await OnSaveItem());
            TogglePasswordVievCommand = new Command(() => HiddingPassword = !HiddingPassword);
            CopyPasswordCommand = new Command(CopyPassword);
        }

        private async void LoadItemFromID(int id)
        {
            Title = "Edit password";
            NewPassword = false;
            var item = await PasswordDatabase.GetItemAsync(id);
            Designation = item.Designation;
            Password = item.Password;
        }

        private async Task OnSaveItem()
        {
            Message = "";

            if (string.IsNullOrEmpty(Designation) || string.IsNullOrEmpty(Password))
            {
                Message = "You need to fill all of the fields";
                return;
            }

            if (NewPassword)
            {
                var pass = await PasswordDatabase.AddItemAsync(new Models.PasswordEntry(Designation, Password));
                if (pass == null)
                {
                    Message = "Password for this designation already exists!";
                    return;
                }
            }
            else
                await PasswordDatabase.UpdateItemAsync(new Models.PasswordEntry(_id, Designation, Password));
            await Shell.Current.GoToAsync($"..?ItemID={ItemID}");
        }

        private async void CopyPassword(object obj)
        {
            await Shell.Current.DisplayAlert("", "Password copied.", "OK");
            await Clipboard.SetTextAsync(Password);
        }
    }
}
