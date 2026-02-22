using System.ComponentModel.DataAnnotations;

namespace ReceptekWebAPI.Models
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Az új felhasználónév kötelező")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "A felhasználónév 3-50 karakter között lehet")]
        public string NewUsername { get; set; } = null!;
    }
}
