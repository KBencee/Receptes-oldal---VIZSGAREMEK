using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;    

namespace ReceptekWebAPI.Models
{
    public class UserProfileImageDto
    {
        [Required]
        public IFormFile Kep { get; set; } = null!;
    }
}
