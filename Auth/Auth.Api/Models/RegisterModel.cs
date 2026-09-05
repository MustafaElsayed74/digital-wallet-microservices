using System.ComponentModel.DataAnnotations;

namespace Auth.Api.Models
{
    public class RegisterModel
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        [Required, MaxLength(50)]
        public string Username { get; set; }
        [Required, MaxLength(128)]
        public string Email { get; set; }
        [Required, MaxLength(256)]
        public string password { get; set; }
    }
}
