namespace ReceptekWebAPI.Models
{
    public class AdminCimkeDto
    {
        public int Id { get; set; }
        public string CimkeNev { get; set; } = null!;
        public int ReceptekSzama { get; set; }
    }
}