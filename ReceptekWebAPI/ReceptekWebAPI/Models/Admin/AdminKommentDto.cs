using System;

namespace ReceptekWebAPI.Models
{
    public class AdminKommentDto
    {
        public Guid Id { get; set; }
        public string Szoveg { get; set; } = null!;
        public DateTime IrtaEkkor { get; set; }
        public string Username { get; set; } = null!;
        public Guid UserId { get; set; }
        public string ReceptNev { get; set; } = null!;
        public Guid ReceptId { get; set; }
    }
}