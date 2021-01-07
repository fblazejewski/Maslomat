using System.Text.Json.Serialization;

namespace Maslomat.Models
{
    class PasswordEntry
    {
        [JsonIgnore]
        public int PassID { get; set; }
        public string Designation { get; set; }
        public string Password { get; set; }

        static int IDCounter = 0;

        public PasswordEntry()
        {
            PassID = IDCounter++;
        }

        public PasswordEntry(int id, string destination, string password)
        {
            PassID = id;
            Designation = destination;
            Password = password;
        }
        public PasswordEntry(string destination, string password)
        {
            PassID = IDCounter++;
            Designation = destination;
            Password = password;
        }
    }
}
