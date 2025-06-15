using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Controllers.Api
{
    /// <summary>
    /// API-controller voor CRUD-bewerkingen op dieren.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalsApiController : ControllerBase
    {
        private readonly ZooContext _context;

        public AnimalsApiController(ZooContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Haalt alle dieren op.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var animals = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .ToListAsync();
            Console.WriteLine(">>> API AnimalsController bereikt!");

            return Ok(animals);
        }

        /// <summary>
        /// Haalt één dier op aan de hand van ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();
            return Ok(animal);
        }

        /// <summary>
        /// Maakt een nieuw dier aan.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Animal animal)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
        }

        /// <summary>
        /// Wijzigt een bestaand dier.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Animal animal)
        {
            if (id != animal.Id) return BadRequest("ID mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Verwijdert een dier.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(a => a.Id == id);
        }
    }
}
