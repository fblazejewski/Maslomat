using System.ComponentModel.DataAnnotations;

namespace MaslomatAPI.Models
{
    public class UserRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
