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
        public async Task<IActionResult> Index(
            string? name,
            string? species,
            int? categoryId,
            Size? size,
            DietaryClass? dietaryClass,
            ActivityPattern? activityPattern,
            SecurityLevel? securityRequirement)
        {
            var animals = _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                animals = animals.Where(a => a.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(species))
                animals = animals.Where(a => a.Species.Contains(species));

            if (categoryId.HasValue)
                animals = animals.Where(a => a.CategoryId == categoryId);

            if (size.HasValue)
                animals = animals.Where(a => a.Size == size);

            if (dietaryClass.HasValue)
                animals = animals.Where(a => a.DietaryClass == dietaryClass);

            if (activityPattern.HasValue)
                animals = animals.Where(a => a.ActivityPattern == activityPattern);

            if (securityRequirement.HasValue)
                animals = animals.Where(a => a.SecurityRequirement == securityRequirement);

            ViewBag.CurrentFilters = new
            {
                name,
                species,
                categoryId,
                size,
                dietaryClass,
                activityPattern,
                securityRequirement
            };

            ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Size = new SelectList(Enum.GetValues(typeof(Size)).Cast<Size>());
            ViewBag.DietaryClass = new SelectList(Enum.GetValues(typeof(DietaryClass)).Cast<DietaryClass>());
            ViewBag.ActivityPattern = new SelectList(Enum.GetValues(typeof(ActivityPattern)).Cast<ActivityPattern>());
            ViewBag.SecurityRequirement = new SelectList(Enum.GetValues(typeof(SecurityLevel)).Cast<SecurityLevel>());


            return View(await animals.ToListAsync());
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


        public IActionResult Sunrise()
        {
            var animals = _context.Animals.ToList();

            var result = animals.Select(a => new
            {
                a.Name,
                a.ActivityPattern,
                Status = a.ActivityPattern switch
                {
                    ActivityPattern.Diurnal => "Wordt wakker",
                    ActivityPattern.Nocturnal => "Gaat slapen",
                    ActivityPattern.Cathemeral => "Altijd actief",
                    _ => "Onbekend"
                }
            });

            return View(result);
        }






        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }

        private void PopulateDropdowns(Animal? animal = null)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", animal?.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name", animal?.EnclosureId);

            ViewBag.Size = new SelectList(Enum.GetValues(typeof(Size)).Cast<Size>());
            ViewBag.DietaryClass = new SelectList(Enum.GetValues(typeof(DietaryClass)).Cast<DietaryClass>());
            ViewBag.ActivityPattern = new SelectList(Enum.GetValues(typeof(ActivityPattern)).Cast<ActivityPattern>());
            ViewBag.SecurityRequirement = new SelectList(Enum.GetValues(typeof(SecurityLevel)).Cast<SecurityLevel>());
        }
    }
}
