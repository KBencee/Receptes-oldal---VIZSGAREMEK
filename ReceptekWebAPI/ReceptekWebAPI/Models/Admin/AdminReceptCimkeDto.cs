namespace ReceptekWebAPI.Models
{
    public class AdminReceptCimkeDto
    {
        public Guid ReceptId { get; set; }
        public int CimkeId { get; set; }
        public string ReceptNev { get; set; } = null!;
        public string CimkeNev { get; set; } = null!;
    }
}