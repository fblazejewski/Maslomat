using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaslomatAPI.Entities
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        public List<PasswordEntity> Passwords { get; set; }

        public User()
        {
        }
    }
}
