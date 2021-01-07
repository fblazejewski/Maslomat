using System.Threading.Tasks;
using Xamarin.Forms;

namespace Maslomat.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public string Message { get; set; }
        public Color MessageColor { get; set; }

        public bool LoggedOut { get; set; } = true;
        public bool LoggedIn => !LoggedOut;

        public LoginViewModel()
        {
        }

        public async Task<bool> LogIn()
        {
            SystemBusy = true;
            Message = "Connecting...";
            MessageColor = Color.Gray;

            (bool success, string message) = await PasswordDatabase.CloudHandler.LogIn(Login, Password);

            Message = message;
            if (success)
            {
                MessageColor = Color.Green;
                await PasswordDatabase.SyncPasswords();
                LoggedOut = false;
            }
            else
            {
                MessageColor = Color.Red;
            }

            SystemBusy = false;
            return await Task.FromResult(success);
        }

        public void LogOut()
        {
            PasswordDatabase.CloudHandler.LogOut();
            Message = "Logged out";
            MessageColor = Color.Gray;
            LoggedOut = true;
        }
    }
}
