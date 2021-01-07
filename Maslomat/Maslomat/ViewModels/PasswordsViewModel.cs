using Maslomat.Models;
using Maslomat.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Maslomat.ViewModels
{
    class PasswordsViewModel : BaseViewModel
    {
        public ObservableCollection<PasswordEntry> Items { get; set; }
        public PasswordEntry SelectedItem { get; set; }

        public Command AddPasswordCommand { get; }
        public Command LoadDatabaseCommand { get; }
        public Command<PasswordEntry> PasswordDetailsCommand { get; }
        public Command<PasswordEntry> CopyPasswordCommand { get; }

        public bool IsBusy { get; set; }

        public PasswordsViewModel()
        {
            Items = new ObservableCollection<PasswordEntry>();
            AddPasswordCommand = new Command(OnAddPassword);
            PasswordDetailsCommand = new Command<PasswordEntry>(OnPasswordDetails);
            LoadDatabaseCommand = new Command(async () => await LoadItems());
            CopyPasswordCommand = new Command<PasswordEntry>(CopyPassword);
        }

        private async void CopyPassword(PasswordEntry entry)
        {
            await Clipboard.SetTextAsync(entry.Password);
            //todo alert?
            await Shell.Current.DisplayAlert("", "Password copied.", "OK");
            Debug.Print($"Copied password: {entry.Password}");
        }

        async Task LoadItems()
        {
            IsBusy = true;

            Items.Clear();

            var items = await PasswordDatabase.GetAllItemsAsync();
            items.OrderBy(x => x.Designation);
            foreach (var item in items)
            {
                Items.Add(item);
            }

            IsBusy = false;
        }

        async void OnAddPassword(object obj)
        {
            Debug.Print("would open add new password creation page...");
            await Shell.Current.GoToAsync(nameof(PasswordEditPage));
        }

        async void OnPasswordDetails(PasswordEntry entry)
        {
            if (entry == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(PasswordDetailPage)}?{nameof(PasswordDetailViewModel.ItemID)}={entry.PassID}");
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }
    }
}
