using Mango.Web.Utility;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class RegistrationRequestDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "The field {0} must be between {2} and {1} characters long.")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Phone Number")]
        [BulgarianPhoneNumber(ErrorMessage = "Invalid Bulgarian phone number.")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Role { get; set; } = SD.RoleCustomer;
    }
}
