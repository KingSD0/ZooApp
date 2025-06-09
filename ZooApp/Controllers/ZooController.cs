using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooApp.Data;
using System.Linq;
using ZooApp.Models;

namespace ZooApp.Controllers
{
    public class ZooController : Controller
    {
        private readonly ZooContext _context;

        public ZooController(ZooContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Geeft per verblijf een overzicht van dieren en hun status bij zonsopkomst,
        /// gebaseerd op hun activiteitspatroon.
        /// </summary>
        /// <returns>
        /// Een view met per verblijf: diernaam, activiteitspatroon en sunrise-status.
        /// </returns>
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
                }).ToList()
            }).ToList();

            return View(result);
        }

        /// <summary>
        /// Geeft per verblijf een overzicht van dieren en hun status bij zonsondergang,
        /// gebaseerd op hun activiteitspatroon.
        /// </summary>
        /// <returns>
        /// Een view met per verblijf: diernaam, activiteitspatroon en sunset-status.
        /// </returns>
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
                }).ToList()
            }).ToList();

            return View(result);
        }


        /// <summary>
        /// Toont per verblijf wat elk dier eet tijdens feeding time, en waarschuwt als prooidieren
        /// zich in hetzelfde verblijf bevinden als roofdieren.
        /// </summary>
        /// <returns>
        /// Een view met per verblijf: diernaam, dieetklasse, voedingsbeschrijving en eventuele waarschuwing.
        /// </returns>
        public IActionResult FeedingTime()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ThenInclude(a => a.Prey)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                Warning = e.Animals.Any(pred =>
                    pred.DietaryClass == DietaryClass.Carnivore &&
                    pred.Prey != null &&
                    pred.Prey.Any(p => e.Animals.Contains(p)))
                    ? " Let op: prooidieren aanwezig bij roofdieren!"
                    : null,
                DietDetails = e.Animals.Select(a => new
                {
                    a.Name,
                    a.DietaryClass,
                    Description = a.GetFeedingDescription()
                })
            });

            return View(result);
        }

        /// <summary>
        /// Controleert per verblijf of het voldoet aan de ruimtevereisten op basis van de dieren die erin verblijven.
        /// </summary>
        /// <returns>
        /// Een view met per verblijf: naam, grootte, aantal dieren, benodigde ruimte en constraintstatus.
        /// </returns>
        public IActionResult CheckConstraints()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                Size = e.Size,
                AnimalCount = e.Animals.Count,
                RequiredSpace = e.Animals.Sum(a => a.SpaceRequirement),
                Status = e.Size >= e.Animals.Sum(a => a.SpaceRequirement)
                    ? "✅ Verblijf voldoet aan alle eisen."
                    : "⚠️ Verblijf heeft onvoldoende ruimte."
            });

            return View(result);
        }


        /// <summary>
        /// Wijst dieren automatisch toe aan verblijven, op basis van opgegeven strategie.
        /// </summary>
        /// <param name="strategy">Strategie: 'nieuw' (verwijdert alles) of 'aanvullen' (gebruikt bestaande verblijven).</param>
        /// <returns>Redirect naar Index met melding of resultaatweergave.</returns>
        [HttpPost]
        public IActionResult AutoAssign(string strategy = "nieuw")
        {
            if (strategy == "nieuw")
            {
                // 1. Verwijder alle bestaande verblijven en ontkoppel dieren
                var allEnclosures = _context.Enclosures.Include(e => e.Animals).ToList();

                foreach (var enclosure in allEnclosures)
                {
                    foreach (var animal in enclosure.Animals)
                    {
                        animal.Enclosure = null;
                    }
                }

                _context.Enclosures.RemoveRange(allEnclosures);
                _context.SaveChanges();
            }

            // 2. Laad dieren zonder verblijf
            var unassignedAnimals = _context.Animals
                .Include(a => a.Prey)
                .Where(a => a.Enclosure == null)
                .ToList();

            // 3. Laad bestaande verblijven (voor aanvullen + overlapcontrole)
            var existingEnclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            // 4. Groepeer dieren (gebruikt voor naam en clustering)
            var grouped = unassignedAnimals
                .GroupBy(a => new
                {
                    Climate = a.Size switch
                    {
                        Size.VeryLarge => Climate.Tropical,
                        Size.Large => Climate.Temperate,
                        _ => Climate.Arctic
                    },
                    Habitat = a.DietaryClass switch
                    {
                        DietaryClass.Carnivore => HabitatType.Forest,
                        DietaryClass.Herbivore => HabitatType.Grassland,
                        DietaryClass.Piscivore => HabitatType.Aquatic,
                        _ => HabitatType.Desert
                    }
                })
                .ToList();

            int createdCount = 0;

            foreach (var group in grouped)
            {
                var animalsInGroup = group.ToList();
                const int MaxPerEnclosure = 4;

                for (int i = 0; i < animalsInGroup.Count; i += MaxPerEnclosure)
                {
                    var subGroup = animalsInGroup.Skip(i).Take(MaxPerEnclosure).ToList();

                    Enclosure? suitable = null;

                    if (strategy == "aanvullen")
                    {
                        // Zoek geschikt bestaand verblijf
                        suitable = existingEnclosures.FirstOrDefault(e =>
                            e.Climate == group.Key.Climate &&
                            e.HabitatType == group.Key.Habitat &&
                            e.SecurityLevel >= subGroup.Max(a => a.SecurityRequirement) &&
                            e.Size - e.Animals.Sum(a => a.SpaceRequirement) >= subGroup.Sum(a => a.SpaceRequirement));
                    }

                    if (suitable != null)
                    {
                        foreach (var animal in subGroup)
                        {
                            suitable.Animals.Add(animal);
                        }
                    }
                    else
                    {
                        // 5. Nieuw verblijf aanmaken
                        string enclosureName = $"{group.Key.Habitat} - {group.Key.Climate} {++createdCount}";

                        var newEnclosure = new Enclosure
                        {
                            Name = enclosureName,
                            Climate = group.Key.Climate,
                            HabitatType = group.Key.Habitat,
                            Size = subGroup.Sum(a => a.SpaceRequirement) + 20, // extra marge
                            SecurityLevel = subGroup.Max(a => a.SecurityRequirement),
                            Animals = new List<Animal>()
                        };

                        foreach (var animal in subGroup)
                        {
                            newEnclosure.Animals.Add(animal);
                        }

                        _context.Enclosures.Add(newEnclosure);
                        existingEnclosures.Add(newEnclosure); // Voeg toe voor hergebruik bij volgende dieren
                    }
                }
            }

            _context.SaveChanges();

            TempData["AutoAssignMessage"] = $"AutoAssign is succesvol uitgevoerd met strategie: '{strategy}'.";
            return RedirectToAction(nameof(Index));
        }

    }
}
