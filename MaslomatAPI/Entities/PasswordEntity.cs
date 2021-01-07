using System.ComponentModel.DataAnnotations;

namespace MaslomatAPI.Entities
{
    public class PasswordEntity
    {
        [Key]
        public int PassID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public string Designation { get; set; }
        [Required]
        public string Password { get; set; }

        public PasswordEntity()
        {
        }
    }
}
