using System;

namespace ReceptekWebAPI.Models
{
    public class AdminLikeDto
    {
        public string Username { get; set; } = null!;
        public Guid UserId { get; set; }
        public string ReceptNev { get; set; } = null!;
        public Guid ReceptId { get; set; }
        public DateTime LikeoltaEkkor { get; set; }
    }
}