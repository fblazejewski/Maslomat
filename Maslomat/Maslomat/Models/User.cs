namespace Maslomat.Models
{
    class UserRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public UserRequest(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
