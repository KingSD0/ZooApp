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


        /// <summary>
        /// Geeft een overzicht van dieren met hun status bij zonsopkomst,
        /// gebaseerd op hun activiteitspatroon.
        /// </summary>
        /// <returns>Een weergave met diernaam, activiteitspatroon en sunrise-status.</returns>
        public IActionResult Sunrise()
        {
            var animals = _context.Animals
                .AsEnumerable()
                .Select(a => new
                {
                    a.Name,
                    a.ActivityPattern,
                    Status = a.GetSunriseStatus()
                });

            return View(animals);
        }

        /// <summary>
        /// Geeft een overzicht van dieren met hun status bij zonsondergang,
        /// gebaseerd op hun activiteitspatroon.
        /// </summary>
        /// <returns>Een weergave met diernaam, activiteitspatroon en sunset-status.</returns>
        public IActionResult Sunset()
        {
            var animals = _context.Animals.ToList();

            var result = animals.Select(a => new
            {
                a.Name,
                a.ActivityPattern,
                Status = a.GetSunsetStatus()
            });

            return View(result);
        }

        /// <summary>
        /// Genereert het voedingsrapport per verblijf met voederinformatie per dier.
        /// </summary>
        /// <remarks>
        /// Maakt gebruik van <c>GetFeedingDescription()</c> uit het <c>Animal</c>-model.
        /// Geeft per verblijf een waarschuwing als roofdieren samen met hun prooidieren gehuisvest zijn.
        /// </remarks>
        /// <returns>Een view met per verblijf de dieetdetails van de dieren en eventuele waarschuwingen.</returns>
        public IActionResult FeedingTime()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ThenInclude(a => a.Prey)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                DietDetails = e.Animals.Select(a => new
                {
                    a.Name,
                    a.DietaryClass,
                    Description = a.GetFeedingDescription()
                }),
                Warning = e.Animals.Any(predator =>
                    predator.DietaryClass == DietaryClass.Carnivore &&
                    predator.Prey != null &&
                    predator.Prey.Any(prey => e.Animals.Contains(prey))
                ) ? " Let op: prooidieren aanwezig bij roofdieren!" : null
            });

            return View(result);
        }


        /// <summary>
        /// Geeft per verblijf een overzicht van de constraintstatus van dieren,
        /// zoals voldoende ruimte en beveiliging.
        /// </summary>
        /// <returns>Een view met constraintinformatie per verblijf en dier.</returns>
        public IActionResult CheckConstraints()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                SecurityLevel = e.SecurityLevel,
                Constraints = e.Animals.Select(a => new
                {
                    a.Name,
                    a.Species,
                    a.SpaceRequirement,
                    a.SecurityRequirement,
                    Status = a.GetConstraintStatus(e) // correcte aanroep
                })
            });

            return View(result);
        }





        /// <summary>
        /// Controleert of een dier met een specifiek ID bestaat in de database.
        /// </summary>
        /// <param name="id">Het ID van het dier.</param>
        /// <returns>True als het dier bestaat, anders false.</returns>
        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }

        /// <summary>
        /// Vult de ViewData en ViewBag met dropdown-opties voor dierformulieren (Create/Edit).
        /// </summary>
        /// <param name="animal">Optioneel: huidig dier om de geselecteerde waarden vooraf in te stellen.</param>
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
