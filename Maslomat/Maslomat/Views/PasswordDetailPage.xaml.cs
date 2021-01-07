using Maslomat.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Maslomat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordDetailPage : ContentPage
    {
        public PasswordDetailPage()
        {
            InitializeComponent();
            BindingContext = new PasswordDetailViewModel();
        }
    }
}