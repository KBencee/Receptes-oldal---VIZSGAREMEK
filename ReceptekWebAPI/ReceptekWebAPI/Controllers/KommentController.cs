using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptekWebAPI.Data;
using ReceptekWebAPI.Entities;
using ReceptekWebAPI.Models;
using System.Security.Claims;

namespace ReceptekWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KommentController : ControllerBase
    {
        private readonly UserDbContext _context;

        public KommentController(UserDbContext context)
        {
            _context = context;
        }

        [HttpPost("recept/{receptId:guid}")]
        [Authorize]
        public async Task<ActionResult<KommentResponseDto>> Create(Guid receptId, [FromBody] KommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var receptExists = await _context.Receptek.AnyAsync(r => r.Id == receptId);
            if (!receptExists)
                return NotFound("Recept nem található");

            var komment = new ReceptKomment
            {
                Id = Guid.NewGuid(),
                Szoveg = dto.Szoveg,
                UserId = userId,
                ReceptId = receptId,
                IrtaEkkor = DateTime.UtcNow
            };

            _context.ReceptKommentek.Add(komment);
            await _context.SaveChangesAsync();

            var createdKomment = await _context.ReceptKommentek
                .Include(k => k.User)
                .FirstOrDefaultAsync(k => k.Id == komment.Id);

            var response = new KommentResponseDto
            {
                Id = createdKomment!.Id,
                Szoveg = createdKomment.Szoveg,
                IrtaEkkor = createdKomment.IrtaEkkor,
                Username = createdKomment.User?.Username ?? "Unknown",
                UserId = createdKomment.UserId,
                SajatKomment = true
            };

            return CreatedAtAction(nameof(GetById), new { id = komment.Id }, response);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<KommentResponseDto>> GetById(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? currentUserId = Guid.TryParse(userIdStr, out var u)? u : null;

            var komment = await _context.ReceptKommentek
                .Include(k => k.User)
                .OrderBy(k => k.IrtaEkkor)
                .ThenBy(k => k.Id)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (komment == null)
                return NotFound("A komment nem található");

            var response = new KommentResponseDto
            {
                Id = komment.Id,
                Szoveg = komment.Szoveg,
                IrtaEkkor = komment.IrtaEkkor,
                Username = komment.User?.Username ?? "Unknown",
                UserId = komment.UserId,
                SajatKomment = currentUserId == komment.UserId
            };

            return Ok(response);
        }

        [HttpGet("recept/{receptId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<KommentResponseDto>>> GetByRecept(Guid receptId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? currentUserId = Guid.TryParse(userIdStr, out var u) ? u : null;

            var kommentek = await _context.ReceptKommentek
                .Include(k => k.User)
                .Where(k => k.ReceptId == receptId)
                .OrderBy(k => k.IrtaEkkor)
                .ThenBy(k => k.Id)
                .ToListAsync();

            var responses = kommentek.Select(k => new KommentResponseDto
            {
                Id = k.Id,
                Szoveg = k.Szoveg,
                IrtaEkkor = k.IrtaEkkor,
                Username = k.User.Username,
                UserId = k.UserId,
                SajatKomment = currentUserId == k.UserId
            }).ToList();

            return Ok(responses);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<KommentResponseDto>> Update(Guid id, [FromBody] KommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var komment = await _context.ReceptKommentek
                .Include(k => k.User)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (komment == null)
                return NotFound("A komment nem található");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (komment.UserId != userId && userRole != "Admin")
                return Forbid("Csak a saját kommentedet szerkesztheted");

            komment.Szoveg = dto.Szoveg;
            await _context.SaveChangesAsync();

            var response = new KommentResponseDto
            {
                Id = komment.Id,
                Szoveg = komment.Szoveg,
                IrtaEkkor = komment.IrtaEkkor,
                Username = komment.User.Username,
                UserId = komment.UserId,
                SajatKomment = true
            };

            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var komment = await _context.ReceptKommentek.FindAsync(id);
            if (komment == null)
                return NotFound("A komment nem található");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (komment.UserId != userId && userRole != "Admin")
                return Forbid("Csak saját kommentet törölhetsz");

            _context.ReceptKommentek.Remove(komment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Komment törölve" });
        }
    }
}
