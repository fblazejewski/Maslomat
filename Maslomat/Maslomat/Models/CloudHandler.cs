using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Maslomat.Models
{
    class CloudHandler
    {
        public bool LoggedIn { get; private set; }

        private HttpClient _client;
        private List<PasswordEntry> _database;

        private HttpContent _body;
        private HttpResponseMessage _response;

        public CloudHandler(List<PasswordEntry> database)
        {
            _client = new HttpClient(Global.HttpClientHandler);
            _database = database;
        }

        private void GenerateBody(string json)
        {
            _body = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }
        private void CleanUp()
        {
            if (_body != null)
                _body.Dispose();
            if (_response != null)
                _response.Dispose();
            _body = null;
            _response = null;
        }

        public async Task<(bool, string)> LogIn(string user, string password)
        {
            Debug.Print("Sending login request...");
            bool result = false;
            string resultMessage = "Unhandled error";

            string json = JsonSerializer.Serialize(new UserRequest(user, password));

            Uri uri = new Uri($"{Global.ApiUrl}/auth");

            GenerateBody(json);
            try
            {
                _response = await _client.PostAsync(uri, _body);
                var content = await _response.Content.ReadAsStringAsync();
                if (_response.IsSuccessStatusCode)
                {
                    //get a token from returned json
                    var jsonResult = JsonDocument.Parse(content);
                    var token = jsonResult.RootElement.GetProperty("token").GetString();
                    Application.Current.Properties["token"] = token;

                    LoggedIn = true;
                    result = true;
                    resultMessage = "Logged in.";
                    Application.Current.Properties["user"] = user;
                    Application.Current.Properties["password"] = password;
                }
                else
                {
                    resultMessage = "Login failed...";
                }
            }
            catch (Exception e)
            {
                resultMessage = "Error while connecting to the cloud.";
                Debug.Print($"EXCEPTION in LogIn: {e}");
            }
            finally
            {
                CleanUp();
            }
            return await Task.FromResult((result, resultMessage));
        }

        public void LogOut()
        {
            Application.Current.Properties.Remove("user");
            Application.Current.Properties.Remove("password");
            Application.Current.Properties.Remove("token");

            LoggedIn = false;
        }

        public async Task<bool> SyncPasswords()
        {
            bool result = false;

            if (LoggedIn)
            {
                string username = Application.Current.Properties["user"].ToString();
                string password = Application.Current.Properties["password"].ToString();
                string token = Application.Current.Properties["token"].ToString();

                string json = JsonSerializer.Serialize(new UserRequest(username, password));

                Uri uri = new Uri($"{Global.ApiUrl}/passwords");

                GenerateBody(json);
                var authHeaderValue = new AuthenticationHeaderValue("bearer", token);
                _client.DefaultRequestHeaders.Authorization = authHeaderValue;

                try
                {
                    _response = await _client.PostAsync(uri, _body);
                    var content = await _response.Content.ReadAsStringAsync();

                    if (_response.IsSuccessStatusCode)
                    {
                        var passwordsJson = JsonDocument.Parse(content);

                        var cloudPasswords = new List<PasswordEntry>();

                        foreach (var item in passwordsJson.RootElement.EnumerateArray())
                        {
                            var desig = item.GetProperty("designation").GetString();
                            var pass = item.GetProperty("password").GetString();

                            cloudPasswords.Add(new PasswordEntry(desig, pass));
                        }

                        await MergeCloudPasswords(cloudPasswords);
                        result = true;
                    }
                    else
                    {
                        throw new Exception($"Error status code: {_response.StatusCode}");
                    }
                }
                catch (Exception e)
                {
                    Debug.Print($"EXCEPTION in SynchPasswords: {e}");
                }
                finally
                {
                    CleanUp();
                }
            }

            return await Task.FromResult(result);
        }

        private async Task MergeCloudPasswords(List<PasswordEntry> cloudPasswords)
        {
            List<PasswordRequest> missingPasswords = new List<PasswordRequest>();

            foreach (var cloudPassword in cloudPasswords)
            {
                //check if password already exists in the local database
                var pass = _database.Find(x => x.Designation == cloudPassword.Designation);
                //if doesnt exist locally - add it
                if (pass == null)
                {
                    _database.Add(cloudPassword);
                }
                //if it exists - store it to override cloud password
                else
                {
                    missingPasswords.Add(new PasswordRequest(pass));
                }
            }

            //check and send missing passwords from local
            foreach (var localPassword in _database)
                if (!cloudPasswords.Contains(localPassword))
                    missingPasswords.Add(new PasswordRequest(localPassword));

            if (missingPasswords.Count > 0)
                await UploadPasswords(missingPasswords);

        }

        public async Task UploadPasswords(List<PasswordRequest> missingPasswords)
        {
            if (LoggedIn)
            {
                string username = Application.Current.Properties["user"].ToString();
                string token = Application.Current.Properties["token"].ToString();

                string json = JsonSerializer.Serialize(new PasswordsUpdateRequest(username, missingPasswords));

                Uri uri = new Uri($"{Global.ApiUrl}/add-password");

                GenerateBody(json);
                var authHeaderValue = new AuthenticationHeaderValue("bearer", token);
                _client.DefaultRequestHeaders.Authorization = authHeaderValue;

                try
                {
                    _response = await _client.PostAsync(uri, _body);
                    if (_response.IsSuccessStatusCode)
                    {
                        Debug.Print("Succesfully uploaded passwords");
                    }
                    else
                    {
                        Debug.Print($"Error when sending passwords: {_response.StatusCode}");
                    }
                }
                catch (Exception e)
                {
                    Debug.Print($"EXCEPTION in UploadPasswords: {e}");
                }
                finally
                {
                    CleanUp();
                }
            }
        }

        public async Task DeletePassword(PasswordRequest passwordRequest)
        {
            if (LoggedIn)
            {
                string username = Application.Current.Properties["user"].ToString();
                //string password = Application.Current.Properties["password"].ToString();
                string token = Application.Current.Properties["token"].ToString();

                string json = JsonSerializer.Serialize(new PasswordsUpdateRequest(username, new List<PasswordRequest> { passwordRequest }));

                Uri uri = new Uri($"{Global.ApiUrl}/remove-password");

                GenerateBody(json);
                var authHeaderValue = new AuthenticationHeaderValue("bearer", token);
                _client.DefaultRequestHeaders.Authorization = authHeaderValue;

                try
                {
                    _response = await _client.PostAsync(uri, _body);
                    if (_response.IsSuccessStatusCode)
                    {
                        Debug.Print("Succesfully deleted password");
                    }
                    else
                    {
                        Debug.Print($"Error when removing password: {_response.StatusCode}");
                    }
                }
                catch (Exception e)
                {
                    Debug.Print($"EXCEPTION in DeletePasswords: {e}");
                }
                finally
                {
                    CleanUp();
                }
            }
        }
    }
}