using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class LoginRequestDTO
    {
        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
