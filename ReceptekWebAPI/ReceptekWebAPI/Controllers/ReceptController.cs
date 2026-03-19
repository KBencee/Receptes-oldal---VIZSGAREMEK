using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptekWebAPI.Data;
using ReceptekWebAPI.Entities;
using ReceptekWebAPI.Models;
using ReceptekWebAPI.Services;
using System.Security.Claims;

namespace ReceptekWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IImageKitService _imageKitService;

        public ReceptController(UserDbContext context, IImageKitService imageKitService)
        {
            _context = context;
            _imageKitService = imageKitService;
        }

        [HttpPost("with-image")]
        [Authorize]
        public async Task<ActionResult<ReceptResponseDto>> CreateWithImage([FromForm] ReceptWithImageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = new Recept
            {
                Id = Guid.NewGuid(),
                Nev = dto.Nev,
                Leiras = dto.Leiras ?? string.Empty,
                Hozzavalok = dto.Hozzavalok ?? string.Empty,
                ElkeszitesiIdo = dto.ElkeszitesiIdo,
                NehezsegiSzint = dto.NehezsegiSzint ?? string.Empty,
                Likes = 0,
                UserId = userId,
                FeltoltveEkkor = DateTime.UtcNow
            };

            if (dto.Kep != null && dto.Kep.Length > 0)
            {
                try
                {
                    var (url, fileId) = await _imageKitService.UploadAsync(dto.Kep);
                    recept.KepUrl = url;
                    recept.KepFileId = fileId;
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = "Kép feltöltési hiba", details = ex.Message });
                }
            }

            _context.Receptek.Add(recept);

            if (dto.CimkeIds != null && dto.CimkeIds.Any())
            {
                var uniqueIds = dto.CimkeIds.Distinct().ToList();

                var existingTagIds = await _context.Cimkek
                    .Where(c => uniqueIds.Contains(c.CimkeId))
                    .Select(c => c.CimkeId)
                    .ToListAsync();

                if (existingTagIds.Count != uniqueIds.Count)
                {
                    return BadRequest("Egy vagy több megadott címke nem létezik.");
                }

                foreach (var cimkeId in uniqueIds)
                {
                    _context.ReceptCimkek.Add(new ReceptCimke
                    {
                        ReceptId = recept.Id,
                        CimkeId = cimkeId
                    });
                }
            }

            await _context.SaveChangesAsync();

            var r = await _context.Receptek
                .Include(x => x.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == recept.Id);

            var response = new ReceptResponseDto
            {
                Id = r!.Id,
                Nev = r.Nev,
                Leiras = r.Leiras,
                Hozzavalok = r.Hozzavalok,
                ElkeszitesiIdo = r.ElkeszitesiIdo,
                NehezsegiSzint = r.NehezsegiSzint,
                Likes = r.Likes,
                FeltoltoUsername = r.User?.Username ?? "Unknown",
                FeltoltveEkkor = r.FeltoltveEkkor,
                KepUrl = r.KepUrl,
                Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = false,
                LikeolvaVan = false
            };

            return CreatedAtAction(nameof(GetById), new { id = recept.Id }, response);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReceptResponseDto>> Create([FromBody] ReceptDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = new Recept
            {
                Id = Guid.NewGuid(),
                Nev = dto.Nev,
                Leiras = dto.Leiras ?? string.Empty,
                Hozzavalok = dto.Hozzavalok ?? string.Empty,
                ElkeszitesiIdo = dto.ElkeszitesiIdo,
                NehezsegiSzint = dto.NehezsegiSzint ?? string.Empty,
                Likes = 0,
                UserId = userId,
                FeltoltveEkkor = DateTime.UtcNow
            };

            _context.Receptek.Add(recept);

            if (dto.CimkeIds != null && dto.CimkeIds.Any())
            {
                var uniqueIds = dto.CimkeIds.Distinct().ToList();

                var existingTagIds = await _context.Cimkek
                    .Where(c => uniqueIds.Contains(c.CimkeId))
                    .Select(c => c.CimkeId)
                    .ToListAsync();

                if (existingTagIds.Count != uniqueIds.Count)
                {
                    return BadRequest("Egy vagy több megadott címke nem létezik.");
                }

                foreach (var cimkeId in uniqueIds)
                {
                    _context.ReceptCimkek.Add(new ReceptCimke
                    {
                        ReceptId = recept.Id,
                        CimkeId = cimkeId
                    });
                }
            }

            await _context.SaveChangesAsync();

            var r = await _context.Receptek
                .Include(x => x.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == recept.Id);

            var response = new ReceptResponseDto
            {
                Id = r!.Id,
                Nev = r.Nev,
                Leiras = r.Leiras,
                Hozzavalok = r.Hozzavalok,
                ElkeszitesiIdo = r.ElkeszitesiIdo,
                NehezsegiSzint = r.NehezsegiSzint,
                Likes = r.Likes,
                FeltoltoUsername = r.User?.Username ?? "Unknown",
                FeltoltveEkkor = r.FeltoltveEkkor,
                KepUrl = r.KepUrl,
                Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = false,
                LikeolvaVan = false
            };

            return CreatedAtAction(nameof(GetById), new { id = recept.Id }, response);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ReceptResponseDto>> GetById(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdStr, out var u) ? u : null;

            var r = await _context.Receptek
                .Include(r => r.User)
                .Include(r => r.ReceptCimkek)
                .ThenInclude(rc => rc.Cimke)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r is null)
                return NotFound("Recept nem található");

            List<Guid> mentettReceptIds = new();
            List<Guid> likeoltReceptIds = new();

            if (userId.HasValue)
            {
                mentettReceptIds = await _context.MentettReceptek
                    .Where(mr => mr.UserId == userId.Value)
                    .Select(mr => mr.ReceptId)
                    .ToListAsync();

                likeoltReceptIds = await _context.Likes
                    .Where(rl => rl.UserId == userId.Value)
                    .Select(rl => rl.ReceptId)
                    .ToListAsync();
            }

            var response = new ReceptResponseDto
            {
                Id = r.Id,
                Nev = r.Nev,
                Leiras = r.Leiras,
                Hozzavalok = r.Hozzavalok,
                ElkeszitesiIdo = r.ElkeszitesiIdo,
                NehezsegiSzint = r.NehezsegiSzint,
                Likes = r.Likes,
                FeltoltoUsername = r.User?.Username ?? "Unknown",
                FeltoltveEkkor = r.FeltoltveEkkor,
                KepUrl = r.KepUrl,
                Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = mentettReceptIds.Contains(r.Id),
                LikeolvaVan = likeoltReceptIds.Contains(r.Id)
            };

            return Ok(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ReceptResponseDto>>> GetAll()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdStr, out var u) ? u : null;

            var recepts = await _context.Receptek
                .Include(r => r.ReceptCimkek)
                    .ThenInclude(rc => rc.Cimke)
                .Include(r => r.User)
                .OrderByDescending(r => r.FeltoltveEkkor)
                .ThenBy(r => r.Id)
                .ToListAsync();

            List<Guid> mentettReceptIds = new();
            List<Guid> likeoltReceptIds = new();

            if (userId.HasValue)
            {
                mentettReceptIds = await _context.MentettReceptek
                    .Where(mr => mr.UserId == userId.Value)
                    .Select(mr => mr.ReceptId)
                    .ToListAsync();

                likeoltReceptIds = await _context.Likes
                    .Where(rl => rl.UserId == userId.Value)
                    .Select(rl => rl.ReceptId)
                    .ToListAsync();
            }

            var responses = recepts.Select(r => new ReceptResponseDto
            {
                Id = r.Id,
                Nev = r.Nev,
                Leiras = r.Leiras,
                Hozzavalok = r.Hozzavalok,
                ElkeszitesiIdo = r.ElkeszitesiIdo,
                NehezsegiSzint = r.NehezsegiSzint,
                Likes = r.Likes,
                FeltoltoUsername = r.User?.Username ?? "Unknown",
                FeltoltveEkkor = r.FeltoltveEkkor,
                KepUrl = r.KepUrl,
                Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = mentettReceptIds.Contains(r.Id),
                LikeolvaVan = likeoltReceptIds.Contains(r.Id)
            }).ToList();

            return Ok(responses);
        }

        [HttpGet("{id:guid}/next")]
        [AllowAnonymous]
        public async Task<ActionResult<ReceptResponseDto>> GetNext(Guid id, [FromQuery] string direction = "next", [FromQuery] int? cimkeId = null)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdStr, out var u) ? u : null;

            var current = await _context.Receptek
                .Include(r => r.ReceptCimkek)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (current == null)
                return NotFound("Recept nem található");

            var baseQuery = _context.Receptek
                .Include(r => r.User)
                .Include(r => r.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                .AsQueryable();

            if (cimkeId.HasValue)
            {
                baseQuery = baseQuery.Where(r => r.ReceptCimkek.Any(rc => rc.CimkeId == cimkeId.Value));
            }

            Recept? candidate = null;

            if (string.Equals(direction, "prev", StringComparison.OrdinalIgnoreCase))
            {
                candidate = await baseQuery
                    .Where(r => r.FeltoltveEkkor == current.FeltoltveEkkor && r.Id.CompareTo(current.Id) > 0)
                    .OrderBy(r => r.Id)
                    .FirstOrDefaultAsync();

                if (candidate == null)
                {
                    candidate = await baseQuery
                        .Where(r => r.FeltoltveEkkor < current.FeltoltveEkkor)
                        .OrderByDescending(r => r.FeltoltveEkkor)
                        .ThenBy(r => r.Id)
                        .FirstOrDefaultAsync();
                }
            }
            else
            {
                candidate = await baseQuery
                    .Where(r => r.FeltoltveEkkor == current.FeltoltveEkkor && r.Id.CompareTo(current.Id) < 0)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefaultAsync();

                if (candidate == null)
                {
                    candidate = await baseQuery
                        .Where(r => r.FeltoltveEkkor > current.FeltoltveEkkor)
                        .OrderBy(r => r.FeltoltveEkkor)
                        .ThenByDescending(r => r.Id)
                        .FirstOrDefaultAsync();
                }
            }

            if (candidate == null)
                return NotFound("Nincs további recept a megadott irányban");

            List<Guid> mentettReceptIds = new();
            List<Guid> likeoltReceptIds = new();

            if (userId.HasValue)
            {
                mentettReceptIds = await _context.MentettReceptek
                    .Where(mr => mr.UserId == userId.Value)
                    .Select(mr => mr.ReceptId)
                    .ToListAsync();

                likeoltReceptIds = await _context.Likes
                    .Where(rl => rl.UserId == userId.Value)
                    .Select(rl => rl.ReceptId)
                    .ToListAsync();
            }

            var response = new ReceptResponseDto
            {
                Id = candidate.Id,
                Nev = candidate.Nev,
                Leiras = candidate.Leiras,
                Hozzavalok = candidate.Hozzavalok,
                ElkeszitesiIdo = candidate.ElkeszitesiIdo,
                NehezsegiSzint = candidate.NehezsegiSzint,
                Likes = candidate.Likes,
                FeltoltoUsername = candidate.User?.Username ?? "Unknown",
                FeltoltveEkkor = candidate.FeltoltveEkkor,
                KepUrl = candidate.KepUrl,
                Cimkek = candidate.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = mentettReceptIds.Contains(candidate.Id),
                LikeolvaVan = likeoltReceptIds.Contains(candidate.Id)
            };

            return Ok(response);
        }

        [HttpGet("my-recipes")]
        [Authorize]
        public async Task<ActionResult<List<ReceptResponseDto>>> GetMyRecipes()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var receptek = await _context.Receptek
                .Where(r => r.UserId == userId)
                .Include(r => r.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                .Include(r => r.User)
                .OrderByDescending(r => r.FeltoltveEkkor)
                .ToListAsync();

            var mentettReceptIds = await _context.MentettReceptek
                .Where(mr => mr.UserId == userId)
                .Select(mr => mr.ReceptId)
                .ToListAsync();

            var likeoltReceptIds = await _context.Likes
                .Where(rl => rl.UserId == userId)
                .Select(rl => rl.ReceptId)
                .ToListAsync();

            var responses = receptek.Select(r => new ReceptResponseDto
            {
                Id = r.Id,
                Nev = r.Nev,
                Leiras = r.Leiras,
                Hozzavalok = r.Hozzavalok,
                ElkeszitesiIdo = r.ElkeszitesiIdo,
                NehezsegiSzint = r.NehezsegiSzint,
                Likes = r.Likes,
                FeltoltoUsername = r.User?.Username ?? "Unknown",
                FeltoltveEkkor = r.FeltoltveEkkor,
                KepUrl = r.KepUrl,
                Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = mentettReceptIds.Contains(r.Id),
                LikeolvaVan = likeoltReceptIds.Contains(r.Id)
            }).ToList();

            return Ok(responses);
        }

        [HttpGet("by-tag/{cimkeId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ReceptResponseDto>>> GetByTag(int cimkeId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdStr, out var u) ? u : null;

            var receptek = await _context.Receptek
                .Include(r => r.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                .Include(r => r.User)
                .Where(r => r.ReceptCimkek.Any(rc => rc.CimkeId == cimkeId))
                .ToListAsync();

            List<Guid> mentettReceptIds = new();
            List<Guid> likeoltReceptIds = new();

            if (userId.HasValue)
            {
                mentettReceptIds = await _context.MentettReceptek
                    .Where(mr => mr.UserId == userId.Value)
                    .Select(mr => mr.ReceptId)
                    .ToListAsync();

                likeoltReceptIds = await _context.Likes
                    .Where(rl => rl.UserId == userId.Value)
                    .Select(rl => rl.ReceptId)
                    .ToListAsync();
            }

            var responses = receptek.Select(r => new ReceptResponseDto
            {
                Id = r.Id,
                Nev = r.Nev,
                Leiras = r.Leiras,
                Hozzavalok = r.Hozzavalok,
                ElkeszitesiIdo = r.ElkeszitesiIdo,
                NehezsegiSzint = r.NehezsegiSzint,
                Likes = r.Likes,
                FeltoltoUsername = r.User?.Username ?? "Unknown",
                FeltoltveEkkor = r.FeltoltveEkkor,
                KepUrl = r.KepUrl,
                Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = mentettReceptIds.Contains(r.Id),
                LikeolvaVan = likeoltReceptIds.Contains(r.Id)
            }).ToList();

            return Ok(responses);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ReceptResponseDto>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Keresési kifejezés kötelező");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdStr, out var u) ? u : null;

            var receptek = await _context.Receptek
                .Include(r => r.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                .Include(r => r.User)
                .Where(r => r.Nev.ToLower().Contains(query) ||
                            r.Leiras.ToLower().Contains(query) ||
                            r.Hozzavalok.ToLower().Contains(query))
                .ToListAsync();

            List<Guid> mentettReceptIds = new();
            List<Guid> likeoltReceptIds = new();

            if (userId.HasValue)
            {
                mentettReceptIds = await _context.MentettReceptek
                    .Where(mr => mr.UserId == userId.Value)
                    .Select(mr => mr.ReceptId)
                    .ToListAsync();

                likeoltReceptIds = await _context.Likes
                    .Where(rl => rl.UserId == userId.Value)
                    .Select(rl => rl.ReceptId)
                    .ToListAsync();
            }

            var responses = receptek.Select(r => new ReceptResponseDto
            {
                Id = r.Id,
                Nev = r.Nev,
                Leiras = r.Leiras,
                Hozzavalok = r.Hozzavalok,
                ElkeszitesiIdo = r.ElkeszitesiIdo,
                NehezsegiSzint = r.NehezsegiSzint,
                Likes = r.Likes,
                FeltoltoUsername = r.User?.Username ?? "Unknown",
                FeltoltveEkkor = r.FeltoltveEkkor,
                KepUrl = r.KepUrl,
                Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = mentettReceptIds.Contains(r.Id),
                LikeolvaVan = likeoltReceptIds.Contains(r.Id)
            }).ToList();

            return Ok(responses);
        }

        [HttpPost("{id:guid}/add-image")]
        [Authorize]
        public async Task<ActionResult<ReceptResponseDto>> AddImage(Guid id, [FromForm] IFormFile kep)
        {
            if (kep == null || kep.Length == 0)
                return BadRequest("Kép fájl kötelező");

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = await _context.Receptek.FirstOrDefaultAsync(r => r.Id == id);

            if (recept == null)
                return NotFound("Recept nem található");

            if (recept.UserId != userId)
                return Forbid("Csak a saját recepted képét módosíthatod");

            if (!string.IsNullOrEmpty(recept.KepUrl))
                return BadRequest("A receptnek már van képe. Használd az update-image endpointot!");

            try
            {
                var (url, fileId) = await _imageKitService.UploadAsync(kep);
                recept.KepUrl = url;
                recept.KepFileId = fileId;
                await _context.SaveChangesAsync();

                var r = await _context.Receptek
                    .Include(x => x.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                var mentettReceptIds = await _context.MentettReceptek
                    .Where(mr => mr.UserId == userId)
                    .Select(mr => mr.ReceptId)
                    .ToListAsync();

                var likeoltReceptIds = await _context.Likes
                    .Where(rl => rl.UserId == userId)
                    .Select(rl => rl.ReceptId)
                    .ToListAsync();

                var response = new ReceptResponseDto
                {
                    Id = r!.Id,
                    Nev = r.Nev,
                    Leiras = r.Leiras,
                    Hozzavalok = r.Hozzavalok,
                    ElkeszitesiIdo = r.ElkeszitesiIdo,
                    NehezsegiSzint = r.NehezsegiSzint,
                    Likes = r.Likes,
                    FeltoltoUsername = r.User?.Username ?? "Unknown",
                    FeltoltveEkkor = r.FeltoltveEkkor,
                    KepUrl = r.KepUrl,
                    Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                    MentveVan = mentettReceptIds.Contains(r.Id),
                    LikeolvaVan = likeoltReceptIds.Contains(r.Id)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Kép feltöltési hiba", details = ex.Message });
            }
        }

        [HttpPut("{id:guid}/update-image")]
        [Authorize]
        public async Task<ActionResult<ReceptResponseDto>> UpdateImage(Guid id, [FromForm] IFormFile kep)
        {
            if (kep == null || kep.Length == 0)
                return BadRequest("Kép fájl kötelező");

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = await _context.Receptek.FirstOrDefaultAsync(r => r.Id == id);

            if (recept == null)
                return NotFound("Recept nem található");

            if (recept.UserId != userId)
                return Forbid("Csak a saját recepted képét módosíthatod");

            try
            {
                if (!string.IsNullOrEmpty(recept.KepFileId))
                {
                    await _imageKitService.DeleteAsync(recept.KepFileId);
                }

                var (url, fileId) = await _imageKitService.UploadAsync(kep);
                recept.KepUrl = url;
                recept.KepFileId = fileId;
                await _context.SaveChangesAsync();

                var r = await _context.Receptek
                    .Include(x => x.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                var mentettReceptIds = await _context.MentettReceptek
                    .Where(mr => mr.UserId == userId)
                    .Select(mr => mr.ReceptId)
                    .ToListAsync();

                var likeoltReceptIds = await _context.Likes
                    .Where(rl => rl.UserId == userId)
                    .Select(rl => rl.ReceptId)
                    .ToListAsync();

                var response = new ReceptResponseDto
                {
                    Id = r!.Id,
                    Nev = r.Nev,
                    Leiras = r.Leiras,
                    Hozzavalok = r.Hozzavalok,
                    ElkeszitesiIdo = r.ElkeszitesiIdo,
                    NehezsegiSzint = r.NehezsegiSzint,
                    Likes = r.Likes,
                    FeltoltoUsername = r.User?.Username ?? "Unknown",
                    FeltoltveEkkor = r.FeltoltveEkkor,
                    KepUrl = r.KepUrl,
                    Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                    MentveVan = mentettReceptIds.Contains(r.Id),
                    LikeolvaVan = likeoltReceptIds.Contains(r.Id)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Kép frissítési hiba", details = ex.Message });
            }
        }

        [HttpDelete("{id:guid}/delete-image")]
        [Authorize]
        public async Task<ActionResult> DeleteImage(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = await _context.Receptek.FirstOrDefaultAsync(r => r.Id == id);

            if (recept == null)
                return NotFound("Recept nem található");

            if (recept.UserId != userId)
                return Forbid("Csak a saját recepted képét törölheted");

            if (string.IsNullOrEmpty(recept.KepUrl))
                return BadRequest("A receptnek nincs képe");

            try
            {
                if (!string.IsNullOrEmpty(recept.KepFileId))
                {
                    await _imageKitService.DeleteAsync(recept.KepFileId);
                }

                recept.KepUrl = null;
                recept.KepFileId = null;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Kép sikeresen törölve" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Kép törlési hiba", details = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = await _context.Receptek.FindAsync(id);
            if (recept == null)
                return NotFound("Recept nem található");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (recept.UserId != userId && userRole != "Admin")
            {
                return Forbid("Ha nem vagy admin, csak a saját recepted törölheted");
            }

            if (!string.IsNullOrEmpty(recept.KepFileId))
            {
                await _imageKitService.DeleteAsync(recept.KepFileId);
            }

            _context.Receptek.Remove(recept);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Recept sikeresen törölve" });
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ReceptResponseDto>> Update(Guid id, [FromBody] ReceptDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = await _context.Receptek.FindAsync(id);
            if (recept == null)
                return NotFound("Recept nem található");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (recept.UserId != userId && userRole != "Admin")
            {
                return Forbid("Ha nem vagy admin, csak a saját recepted szerkesztheted");
            }

            recept.Nev = dto.Nev;
            recept.Leiras = dto.Leiras ?? string.Empty;
            recept.Hozzavalok = dto.Hozzavalok ?? string.Empty;
            recept.ElkeszitesiIdo = dto.ElkeszitesiIdo;
            recept.NehezsegiSzint = dto.NehezsegiSzint ?? string.Empty;

            var meglevoReceptCimkek = await _context.ReceptCimkek
                .Where(rc => rc.ReceptId == id)
                .ToListAsync();
            _context.ReceptCimkek.RemoveRange(meglevoReceptCimkek);

            if (dto.CimkeIds != null && dto.CimkeIds.Any())
            {
                foreach (var cimkeId in dto.CimkeIds.Distinct())
                {
                    _context.ReceptCimkek.Add(new ReceptCimke
                    {
                        ReceptId = recept.Id,
                        CimkeId = cimkeId
                    });
                }
            }

            await _context.SaveChangesAsync();

            var r = await _context.Receptek
                .Include(x => x.ReceptCimkek).ThenInclude(rc => rc.Cimke)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            var response = new ReceptResponseDto
            {
                Id = r!.Id,
                Nev = r.Nev,
                Leiras = r.Leiras,
                Hozzavalok = r.Hozzavalok,
                ElkeszitesiIdo = r.ElkeszitesiIdo,
                NehezsegiSzint = r.NehezsegiSzint,
                Likes = r.Likes,
                FeltoltoUsername = r.User?.Username ?? "Unknown",
                FeltoltveEkkor = r.FeltoltveEkkor,
                KepUrl = r.KepUrl,
                Cimkek = r.ReceptCimkek.Select(rc => rc.Cimke.CimkeNev).ToList(),
                MentveVan = false
            };

            return Ok(response);
        }

        [HttpPost("{id:guid}/like")]
        [Authorize]
        public async Task<ActionResult> LikeRecept(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = await _context.Receptek.FindAsync(id);
            if (recept == null)
                return NotFound("Recept nem található");

            var alreadyLiked = await _context.Likes
                .AnyAsync(rl => rl.UserId == userId && rl.ReceptId == id);

            if (alreadyLiked)
                return BadRequest("Ezt a receptet már likeoltad");

            _context.Likes.Add(new Like
            {
                UserId = userId,
                ReceptId = id,
                LikeoltaEkkor = DateTime.UtcNow
            });

            recept.Likes++;
            await _context.SaveChangesAsync();

            var dto = new LikeResponseDto
            {
                Likes = recept.Likes,
                Liked = true
            };

            return Ok(dto);
        }

        [HttpDelete("{id:guid}/like")]
        [Authorize]
        public async Task<ActionResult> UnlikeRecept(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var recept = await _context.Receptek.FindAsync(id);
            if (recept == null)
                return NotFound("Recept nem található");

            var like = await _context.Likes
                .FirstOrDefaultAsync(rl => rl.UserId == userId && rl.ReceptId == id);

            if (like == null)
                return BadRequest("Nem likeoltad ezt a receptet");

            _context.Likes.Remove(like);

            if (recept.Likes > 0)
                recept.Likes--;

            await _context.SaveChangesAsync();

            var dto = new LikeResponseDto
            {
                Likes = recept.Likes,
                Liked = false
            };

            return Ok(dto);
        }
    }
}
