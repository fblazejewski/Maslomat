using Maslomat.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Maslomat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        RegisterViewModel _vm;

        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = _vm = new RegisterViewModel();
        }

        private async void OnRegister(object sender, EventArgs e)
        {
            if (await _vm.Register())
            {
                //wait for a while?
                await Navigation.PopModalAsync();
            }
        }
    }
}