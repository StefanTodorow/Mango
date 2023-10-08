using Mango.Web.Utility;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class CartHeaderDTO
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "The field {0} must be between {2} and {1} characters long.")]
        public string? Name { get; set; }
        [Required]
        [DisplayName("Phone Number")]
        [BulgarianPhoneNumber(ErrorMessage = "Invalid Bulgarian phone number.")]
        public string? PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
