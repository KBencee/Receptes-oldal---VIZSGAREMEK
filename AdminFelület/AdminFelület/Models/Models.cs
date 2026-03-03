using System;
using System.Collections.Generic;
using System.Text;

namespace AdminFelület.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserModel User { get; set; } = null!;
    }

    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfilKepUrl { get; set; }
    }

    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }

    public class ReceptModel
    {
        public Guid Id { get; set; }
        public string Nev { get; set; } = string.Empty;
        public string Leiras { get; set; } = string.Empty;
        public string Hozzavalok { get; set; } = string.Empty;
        public int ElkeszitesiIdo { get; set; }
        public string NehezsegiSzint { get; set; } = string.Empty;
        public int Likes { get; set; }
        public string FeltoltoUsername { get; set; } = string.Empty;
        public DateTime FeltoltveEkkor { get; set; }
        public string? KepUrl { get; set; }
    }

    public class KommentModel
    {
        public Guid Id { get; set; }
        public string Szoveg { get; set; } = string.Empty;
        public DateTime IrtaEkkor { get; set; }
        public string Username { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class StatisztikaModel
    {
        public int OsszFelhasznalo { get; set; }
        public int OsszRecept { get; set; }
        public int OsszKomment { get; set; }
        public int OsszLike { get; set; }
        public int ReceptekKeppel { get; set; }
        public int ReceptekKepNelkul { get; set; }
    }
}
