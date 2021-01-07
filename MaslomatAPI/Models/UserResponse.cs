using MaslomatAPI.Entities;

namespace MaslomatAPI.Models
{
    public class UserResponse
    {
        public string Name { get; set; }
        public string Token { get; set; }

        public UserResponse(User user, string token)
        {
            Name = user.Name;
            Token = token;
        }
    }
}
