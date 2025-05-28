using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly ZooContext _context;

        public AnimalsController(ZooContext context)
        {
            _context = context;
        }

        // GET: Animals
        public async Task<IActionResult> Index()
        {
            var zooContext = _context.Animals.Include(a => a.Category).Include(a => a.Enclosure);
            return View(await zooContext.ToListAsync());
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null) return NotFound();

            return View(animal);
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Animals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Species,CategoryId,Size,DietaryClass,ActivityPattern,EnclosureId,SpaceRequirement,SecurityRequirement")] Animal animal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(animal);
            return View(animal);
        }

        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            PopulateDropdowns(animal);
            return View(animal);
        }

        // POST: Animals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,CategoryId,Size,DietaryClass,ActivityPattern,EnclosureId,SpaceRequirement,SecurityRequirement")] Animal animal)
        {
            if (id != animal.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(animal.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(animal);
            return View(animal);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null) return NotFound();

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null) _context.Animals.Remove(animal);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }

        // Dropdown-helper
        private void PopulateDropdowns(Animal? animal = null)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", animal?.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name", animal?.EnclosureId);

            ViewBag.Size = new SelectList(Enum.GetValues(typeof(Size)));
            ViewBag.DietaryClass = new SelectList(Enum.GetValues(typeof(DietaryClass)));
            ViewBag.ActivityPattern = new SelectList(Enum.GetValues(typeof(ActivityPattern)));
            ViewBag.SecurityRequirement = new SelectList(Enum.GetValues(typeof(SecurityLevel)));
        }
    }
}
