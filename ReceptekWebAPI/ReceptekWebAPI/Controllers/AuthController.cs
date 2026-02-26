using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReceptekWebAPI.Data;
using ReceptekWebAPI.Entities;
using ReceptekWebAPI.Models;
using ReceptekWebAPI.Services;

namespace ReceptekWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserDbContext _context;
        private readonly IImageKitService _imageKitService;

        public AuthController(IAuthService authService, UserDbContext context, IImageKitService imageKitService)
        {
            _authService = authService;
            _context = context;
            _imageKitService = imageKitService;
        }

        //User endpointok

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserDto request)
        {
            var user = await _authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("User already exists.");
            }

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetMe()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(username))
                return Unauthorized();

            if (!System.Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (userEntity == null)
                return Unauthorized();

            var response = new UserResponseDto
            {
                Id = userEntity.Id,
                Username = userEntity.Username,
                Role = userEntity.Role,
                ProfileImageUrl = userEntity.ProfileImageUrl
            };

            return Ok(response);
        }

        [HttpPut("me/username")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> UpdateUsername([FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Felhasználó nem található");

            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == dto.NewUsername && u.Id != userId);

            if (usernameExists)
                return BadRequest("Ez a felhasználónév már foglalt");

            user.Username = dto.NewUsername;
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return Ok(response);
        }

        [HttpPut("me/password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] PasswordChangeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Felhasználó nem található");

            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.OldPassword
            );

            if (verificationResult == PasswordVerificationResult.Failed)
                return BadRequest("A régi jelszó nem megfelelő");

            user.PasswordHash = passwordHasher.HashPassword(user, dto.NewPassword);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Jelszó sikeresen megváltoztatva. Jelentkezz be újra!" });
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role,
                    ProfileImageUrl = u.ProfileImageUrl
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpPost("profilkep")]
        public async Task<ActionResult<UserResponseDto>> UploadProfilKep([FromForm] UserProfileImageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var file = dto.Kep;
            if (file == null || file.Length == 0)
                return BadRequest("File required.");

            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(file.ContentType))
                return BadRequest("Invalid image type. Allowed: jpeg, png, webp.");

            const long maxSize = 5 * 1024 * 1024;
            if (file.Length > maxSize)
                return BadRequest("File too large (max 5 MB).");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !System.Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            try
            {
                var (url, fileId) = await _imageKitService.UploadAsync(file);

                if (!string.IsNullOrEmpty(user.ProfileImageFileId))
                {
                    try { await _imageKitService.DeleteAsync(user.ProfileImageFileId); } catch { }
                }

                user.ProfileImageUrl = url;
                user.ProfileImageFileId = fileId;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var response = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    ProfileImageUrl = user.ProfileImageUrl
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Image upload failed", details = ex.Message });
            }
        }

        //Admin endpointok

        [HttpGet("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role,
                    ProfileImageUrl = u.ProfileImageUrl
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("admin/users/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> AdminGetUserById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Felhasználó nem található");

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return Ok(response);
        }

        [HttpPut("admin/users/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(Guid id, [FromBody] AdminUserUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Felhasználó nem található");

            if (!string.IsNullOrEmpty(dto.Username))
            {
                var usernameExists = await _context.Users
                    .AnyAsync(u => u.Username == dto.Username && u.Id != id);

                if (usernameExists)
                    return BadRequest("Ez a felhasználónév már foglalt");

                user.Username = dto.Username;
            }

            if (!string.IsNullOrEmpty(dto.Role))
            {
                if (dto.Role != "User" && dto.Role != "Admin")
                    return BadRequest("A role csak 'User' vagy 'Admin' lehet");

                user.Role = dto.Role;
            }

            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return Ok(response);
        }

        [HttpPut("admin/users/{id:guid}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeUserRole(Guid id, [FromBody] string newRole)
        {
            if (newRole != "User" && newRole != "Admin")
                return BadRequest("A rang csak 'User' vagy 'Admin' lehet");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Felhasználó nem található");

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Rang frissítve: {newRole}" });
        }

        [HttpDelete("admin/users/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Felhasználó nem található");

            var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(currentUserIdStr, out var currentUserId) && currentUserId == id)
                return BadRequest("Saját magadat nem törölheted!");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Felhasználó törölve" });
        }

        [HttpGet("admin/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetAdminStats()
        {
            var stats = new
            {
                osszFelhasznalo = await _context.Users.CountAsync(),
                osszRecept = await _context.Receptek.CountAsync(),
                osszKomment = await _context.ReceptKommentek.CountAsync(),

                receptekKeppel = await _context.Receptek.CountAsync(r => r.KepUrl != null),
                receptekKepNelkul = await _context.Receptek.CountAsync(r => r.KepUrl == null),

                osszLike = await _context.Receptek.SumAsync(r => r.Likes),

                legtobbetFeltoltok = await _context.Users
                .Select(u => new
                {
                    username = u.Username,
                    receptekSzama = u.Receptek.Count
                })
                .OrderByDescending(x => x.receptekSzama)
                .Take(5)
                .ToListAsync(),

                topReceptek = await _context.Receptek
                .OrderByDescending(r => r.Likes)
                .Take(5)
                .Select(r => new
                {
                    nev = r.Nev,
                    likes = r.Likes,
                    feltolto = r.User!.Username
                })
                .ToListAsync(),

                maFeltolve = await _context.Receptek
                .CountAsync(r => r.FeltoltveEkkor.Date == DateTime.UtcNow.Date),

                maiKommentek = await _context.ReceptKommentek
                .CountAsync(r => r.IrtaEkkor.Date == DateTime.UtcNow.Date)
            };
            return Ok(stats);
        }
    }
}
