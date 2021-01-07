using Maslomat.Models;
using Maslomat.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Maslomat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            DependencyService.Register<PasswordDatabase>();
        }

        protected override void OnStart()
        {
            Task.Run(async () =>
            {
                var repo = DependencyService.Get<PasswordDatabase>();
                await repo.InitializeDBAsync();
            });
            Global.HttpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
