using Maslomat.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Maslomat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordsPage : ContentPage
    {
        PasswordsViewModel _vm;

        public PasswordsPage()
        {
            InitializeComponent();
            BindingContext = _vm = new PasswordsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();
        }
    }
}