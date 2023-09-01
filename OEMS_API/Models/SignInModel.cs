using System.ComponentModel.DataAnnotations;

namespace OEMS_API.Models
{
    public class SignInModel
    {
        [Required]
        public string Mail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
