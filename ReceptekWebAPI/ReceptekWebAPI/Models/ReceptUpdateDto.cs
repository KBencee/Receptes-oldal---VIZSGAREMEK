namespace ReceptekWebAPI.Models
{
    public class ReceptUpdateDto
    {
        public string? Nev { get; set; }
        public string? Leiras { get; set; }
        public string? Hozzavalok { get; set; }
        public int ElkeszitesiIdo { get; set; }
        public string? NehezsegiSzint { get; set; }
    }
}
