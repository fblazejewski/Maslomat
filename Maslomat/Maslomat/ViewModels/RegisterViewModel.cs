using Maslomat.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Maslomat.ViewModels
{
    class RegisterViewModel : BaseViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }
        public Color MessageColor { get; set; }

        HttpClient _client;

        public RegisterViewModel()
        {
            _client = new HttpClient(Global.HttpClientHandler);
        }

        public async Task<bool> Register()
        {
            SystemBusy = true;

            bool result = false;
            Message = "Registering...";
            MessageColor = Color.Gray;

            if (Login.Length < 4)
            {
                Message = "Login needs at least 4 characters";
                SystemBusy = true;
                return false;
            }
            if (Password.Length < 5)
            {
                Message = "Password needs at least 5 characters";
                SystemBusy = true;
                return false;
            }

            Uri uri = new Uri($"{Global.ApiUrl}/register");

            string json = JsonSerializer.Serialize(new UserRequest(Login, Password));
            HttpContent body = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;

            try
            {
                response = await _client.PostAsync(uri, body);
                if (response.IsSuccessStatusCode)
                {
                    Message = $"Succesfully created user: {Login}";
                    MessageColor = Color.Green;
                    result = true;
                }
                else
                {
                    Message = $"Username already taken";
                    MessageColor = Color.Red;
                }
            }
            catch (Exception e)
            {
                Message = $"Error while connecting: {e}";
                MessageColor = Color.Red;
            }
            finally
            {
                body.Dispose();
                if (response != null)
                    response.Dispose();
                SystemBusy = false;
            }

            return result;
        }
    }
}
