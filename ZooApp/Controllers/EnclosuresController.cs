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
    public class EnclosuresController : Controller
    {
        private readonly ZooContext _context;

        public EnclosuresController(ZooContext context)
        {
            _context = context;
        }

        // GET: Enclosures
        public async Task<IActionResult> Index()
        {
            return View(await _context.Enclosures.ToListAsync());
        }

        // GET: Enclosures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var enclosure = await _context.Enclosures.FirstOrDefaultAsync(m => m.Id == id);
            if (enclosure == null) return NotFound();

            return View(enclosure);
        }

        // GET: Enclosures/Create
        public IActionResult Create()
        {
            PopulateEnumDropdowns();
            return View(new Enclosure());
        }

        // POST: Enclosures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enclosure enclosure, int[] habitatValues)
        {
            enclosure.HabitatType = 0;
            foreach (var val in habitatValues)
            {
                enclosure.HabitatType |= (HabitatType)val;
            }

            if (ModelState.IsValid)
            {
                _context.Add(enclosure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateEnumDropdowns();
            return View(enclosure);
        }

        // GET: Enclosures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure == null) return NotFound();

            PopulateEnumDropdowns();
            return View(enclosure);
        }

        // POST: Enclosures/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Enclosure enclosure, int[] habitatValues)
        {
            if (id != enclosure.Id) return NotFound();

            enclosure.HabitatType = habitatValues
                .Select(h => (HabitatType)h)
                .Aggregate(HabitatType.None, (current, next) => current | next);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enclosure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnclosureExists(enclosure.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateEnumDropdowns();
            return View(enclosure);
        }

        // GET: Enclosures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enclosure = await _context.Enclosures.FirstOrDefaultAsync(m => m.Id == id);
            if (enclosure == null) return NotFound();

            return View(enclosure);
        }

        // POST: Enclosures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure != null)
            {
                _context.Enclosures.Remove(enclosure);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Geeft per verblijf een overzicht van dieren en hun status bij zonsopkomst,
        /// gebaseerd op hun activiteitspatroon.
        /// </summary>
        /// <returns>Een weergave met verblijfnaam, diergegevens en sunrise-status.</returns>
        public IActionResult Sunrise()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                SunriseStatuses = e.Animals.Select(a => new
                {
                    a.Name,
                    a.ActivityPattern,
                    Status = a.GetSunriseStatus()
                })
            });

            return View(result);
        }


        /// <summary>
        /// Geeft per verblijf een overzicht van dieren en hun status bij zonsondergang,
        /// gebaseerd op hun activiteitspatroon.
        /// </summary>
        /// <returns>Een weergave met verblijfnaam, diergegevens en sunset-status.</returns>
        public IActionResult Sunset()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                SunsetStatuses = e.Animals.Select(a => new
                {
                    a.Name,
                    a.ActivityPattern,
                    Status = a.GetSunsetStatus()
                })
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










        private bool EnclosureExists(int id)
        {
            return _context.Enclosures.Any(e => e.Id == id);
        }

        private void PopulateEnumDropdowns()
        {
            ViewBag.Climate = new SelectList(Enum.GetValues(typeof(Climate)).Cast<Climate>());
            ViewBag.SecurityLevel = new SelectList(Enum.GetValues(typeof(SecurityLevel)).Cast<SecurityLevel>());

            // Filter 'None' uit de lijst zodat het niet weergegeven wordt in multiselect
            ViewBag.HabitatType = Enum.GetValues(typeof(HabitatType))
                .Cast<HabitatType>()
                .Where(h => h != HabitatType.None)
                .ToList();
        }
    }
}
