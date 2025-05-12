using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergy_ConnectApp.Models
{
    public class User
    {
        public int UserId { get; set; } // Primary Key

        public string FullName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "User type is required.")]
        public string UserType { get; set; }
    }
}
