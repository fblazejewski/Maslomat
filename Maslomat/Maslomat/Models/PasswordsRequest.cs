using System.Collections.Generic;

namespace Maslomat.Models
{
    class PasswordsUpdateRequest
    {
        public string Username { get; set; }
        public List<PasswordRequest> Passwords { get; set; }

        public PasswordsUpdateRequest(string username, List<PasswordRequest> passwords)
        {
            Username = username;
            Passwords = passwords;
        }
    }

    class PasswordRequest
    {
        public string Designation { get; set; }
        public string Password { get; set; }

        public PasswordRequest(PasswordEntry entry)
        {
            Designation = entry.Designation;
            Password = entry.Password;
        }
    }
}
