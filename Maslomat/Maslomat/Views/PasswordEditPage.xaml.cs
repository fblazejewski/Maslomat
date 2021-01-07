using Maslomat.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Maslomat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordEditPage : ContentPage
    {
        public PasswordEditPage()
        {
            InitializeComponent();
            BindingContext = new PasswordEditViewModel();
        }
    }
}