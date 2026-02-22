using System.ComponentModel.DataAnnotations;

namespace ReceptekWebAPI.Models
{
    public class PasswordChangeDto
    {
        [Required(ErrorMessage = "A régi jelszó megadása kötelező")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "Az új jelszó megadása kötelező")]
        [StringLength(100, ErrorMessage = "A jelszó maximum 100 karakteres lehet")]
        public string NewPassword { get; set; } = null!;
    }
}
