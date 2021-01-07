using Maslomat.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Maslomat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        LoginViewModel _vm;

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = _vm = new LoginViewModel();
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            if (await _vm.LogIn())
            {
                //await Navigation.PopModalAsync();
            }
            else
            {
            }
        }

        private void OnRegister(object sender, EventArgs e)
        {
            var registerPage = new RegisterPage();
            Navigation.PushModalAsync(registerPage);
        }

        private void OnLogOut(object sender, EventArgs e)
        {
            _vm.LogOut();
        }
    }
}