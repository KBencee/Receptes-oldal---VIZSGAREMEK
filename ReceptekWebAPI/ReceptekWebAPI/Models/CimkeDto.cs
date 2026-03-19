using System.ComponentModel.DataAnnotations;

namespace ReceptekWebAPI.Models
{
    public class CimkeDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string CimkeNev { get; set; } = string.Empty;
        public int CimkeId { get; set; }
    }

    public class ReceptCimkeDto
    {
        public Guid ReceptId { get; set; }
        public int CimkeId { get; set; }
    }
}
