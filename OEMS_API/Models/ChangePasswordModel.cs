using System.ComponentModel.DataAnnotations;

namespace OEMS_API.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Mevcut şifre gereklidir.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
