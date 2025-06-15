using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnclosuresApiController : ControllerBase
    {
        private readonly ZooContext _context;

        public EnclosuresApiController(ZooContext context)
        {
            _context = context;
        }

        // GET: api/EnclosuresApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enclosure>>> GetEnclosures()
        {
            return await _context.Enclosures
                .Include(e => e.Animals)
                .ToListAsync();
        }

        // GET: api/EnclosuresApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Enclosure>> GetEnclosure(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null)
                return NotFound();

            return enclosure;
        }

        // POST: api/EnclosuresApi
        [HttpPost]
        public async Task<ActionResult<Enclosure>> PostEnclosure(Enclosure enclosure)
        {
            _context.Enclosures.Add(enclosure);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnclosure), new { id = enclosure.Id }, enclosure);
        }

        // PUT: api/EnclosuresApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnclosure(int id, Enclosure enclosure)
        {
            if (id != enclosure.Id)
                return BadRequest();

            _context.Entry(enclosure).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Enclosures.Any(e => e.Id == id))
                    return NotFound();

                throw;
            }
        }

        // DELETE: api/EnclosuresApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnclosure(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure == null)
                return NotFound();

            _context.Enclosures.Remove(enclosure);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
