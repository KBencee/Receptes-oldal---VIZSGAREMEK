using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptekWebAPI.Data;
using ReceptekWebAPI.Entities;
using ReceptekWebAPI.Models;

namespace ReceptekWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CimkekController : ControllerBase
    {
        private readonly UserDbContext _context;
        public CimkekController(UserDbContext context) => _context = context;


        public class CimkeCreateDto
        {
            public string CimkeNev { get; set; } = null!;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CimkeDto>> Create([FromBody] CimkeCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cimke = new Cimke { CimkeNev = dto.CimkeNev };
            _context.Cimkek.Add(cimke);
            await _context.SaveChangesAsync();

            var response = new CimkeDto
            {
                CimkeId = cimke.CimkeId,
                CimkeNev = cimke.CimkeNev
            };
            return CreatedAtAction(nameof(GetById), new { id = cimke.CimkeId }, response);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<CimkeDto>> GetById(int id)
        {
            var c = await _context.Cimkek.FindAsync(id);
            if (c is null) return NotFound();
            var dto = new CimkeDto
            {
                CimkeId = c.CimkeId,
                CimkeNev = c.CimkeNev
            };
            return Ok(dto);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<CimkeDto>>> GetAll()
        {
            var cimkek = await _context.Cimkek.ToListAsync();
            var dtos = cimkek.Select(c => new CimkeDto
            {
                CimkeId = c.CimkeId,
                CimkeNev = c.CimkeNev
            }).ToList();
            return Ok(dtos);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var cimke = await _context.Cimkek.FindAsync(id);
            if (cimke == null)
                return NotFound("Címke nem található");

            var kapcsolatok = await _context.ReceptCimkek
                .Where(rc => rc.CimkeId == id)
                .ToListAsync();

            if (kapcsolatok.Any())
            {
                _context.ReceptCimkek.RemoveRange(kapcsolatok);
            }

            _context.Cimkek.Remove(cimke);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Címke sikeresen törölve" });
        }
    }
}
