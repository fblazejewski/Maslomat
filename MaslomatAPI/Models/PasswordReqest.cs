using System.Collections.Generic;

namespace MaslomatAPI.Models
{
    public class PasswordsUpdateRequest
    {
        public string Username { get; set; }
        public List<PasswordRequest> Passwords { get; set; }
    }

    public class PasswordRequest
    {
        public string Designation { get; set; }
        public string Password { get; set; }
    }
}