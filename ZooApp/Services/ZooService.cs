using Microsoft.EntityFrameworkCore;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    /// <summary>
    /// Service-implementatie voor dierentuinacties zoals Sunrise, Sunset, FeedingTime en AutoAssign.
    /// </summary>
    public class ZooService : IZooService
    {
        private readonly ZooContext _context;

        public ZooService(ZooContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Haalt voor elk verblijf op welke dieren wakker zijn bij zonsopkomst.
        /// </summary>
        public async Task<object> GetSunriseOverviewAsync()
        {
            var enclosures = await _context.Enclosures.Include(e => e.Animals).ToListAsync();

            return enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                SunriseStatuses = e.Animals.Select(a => new
                {
                    a.Name,
                    a.ActivityPattern,
                    Status = a.GetSunriseStatus()
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// Haalt voor elk verblijf op welke dieren wakker zijn bij zonsondergang.
        /// </summary>
        public async Task<object> GetSunsetOverviewAsync()
        {
            var enclosures = await _context.Enclosures.Include(e => e.Animals).ToListAsync();

            return enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                SunsetStatuses = e.Animals.Select(a => new
                {
                    a.Name,
                    a.ActivityPattern,
                    Status = a.GetSunsetStatus()
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// Berekent per verblijf wat de dieren eten en of er risico is op roofdieren en prooidieren samen.
        /// </summary>
        public async Task<object> GetFeedingOverviewAsync()
        {
            var enclosures = await _context.Enclosures
                .Include(e => e.Animals)
                .ThenInclude(a => a.Prey)
                .ToListAsync();

            return enclosures.Select(e => new
            {
                EnclosureName = e.Name,

                // Toont waarschuwing als carnivoor zijn prooi deelt in hetzelfde verblijf
                Warning = e.Animals
                    .Where(a => a.DietaryClass == DietaryClass.Carnivore && a.Prey != null)
                    .SelectMany(a => a.Prey
                        .Where(p => e.Animals.Any(ea => ea.Id == p.Id))
                        .Select(p => $"⚠️ {a.Name} deelt verblijf met prooi {p.Name}")
                    )
                    .FirstOrDefault(), // Toon maximaal één waarschuwing per verblijf

                DietDetails = e.Animals.Select(a => new
                {
                    a.Name,
                    a.DietaryClass,
                    Description = a.GetFeedingDescription()
                })
            }).ToList();
        }


        /// <summary>
        /// Controleert per verblijf of de ruimte voldoende is voor het aantal en type dieren.
        /// </summary>
        public async Task<object> GetConstraintOverviewAsync()
        {
            var enclosures = await _context.Enclosures.Include(e => e.Animals).ToListAsync();

            return enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                Size = e.Size,
                AnimalCount = e.Animals.Count,
                RequiredSpace = e.Animals.Sum(a => a.SpaceRequirement),
                Status = e.Size >= e.Animals.Sum(a => a.SpaceRequirement)
                    ? "✅ Voldoet aan eisen"
                    : "⚠️ Onvoldoende ruimte"
            }).ToList();
        }

        /// <summary>
        /// Wijst dieren automatisch toe aan verblijven, afhankelijk van de gekozen strategie ('nieuw' of 'aanvullen').
        /// </summary>
        /// <param name="strategy">Strategie: 'nieuw' (alles opnieuw) of 'aanvullen' (bestaande aanvullen).</param>
        public async Task AutoAssignAsync(string strategy)
        {
            if (strategy == "nieuw")
            {
                var allEnclosures = await _context.Enclosures.Include(e => e.Animals).ToListAsync();
                foreach (var enclosure in allEnclosures)
                    foreach (var animal in enclosure.Animals)
                        animal.Enclosure = null;

                _context.Enclosures.RemoveRange(allEnclosures);
                await _context.SaveChangesAsync();
            }

            var unassignedAnimals = await _context.Animals
                .Include(a => a.Prey)
                .Where(a => a.Enclosure == null)
                .ToListAsync();

            var existingEnclosures = await _context.Enclosures
                .Include(e => e.Animals)
                .ToListAsync();

            var grouped = unassignedAnimals.GroupBy(a => new
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
            }).ToList();

            int createdCount = 0;
            const int MaxPerEnclosure = 4;

            foreach (var group in grouped)
            {
                var animalsInGroup = group.ToList();

                for (int i = 0; i < animalsInGroup.Count; i += MaxPerEnclosure)
                {
                    var subGroup = animalsInGroup.Skip(i).Take(MaxPerEnclosure).ToList();
                    Enclosure? suitable = null;

                    if (strategy == "aanvullen")
                    {
                        suitable = existingEnclosures.FirstOrDefault(e =>
                            e.Climate == group.Key.Climate &&
                            e.HabitatType == group.Key.Habitat &&
                            e.SecurityLevel >= subGroup.Max(a => a.SecurityRequirement) &&
                            e.Size - e.Animals.Sum(a => a.SpaceRequirement) >= subGroup.Sum(a => a.SpaceRequirement));
                    }

                    if (suitable != null)
                    {
                        foreach (var animal in subGroup)
                            suitable.Animals.Add(animal);
                    }
                    else
                    {
                        string name = $"{group.Key.Habitat} - {group.Key.Climate} {++createdCount}";

                        var newEnclosure = new Enclosure
                        {
                            Name = name,
                            Climate = group.Key.Climate,
                            HabitatType = group.Key.Habitat,
                            Size = subGroup.Sum(a => a.SpaceRequirement) + 20,
                            SecurityLevel = subGroup.Max(a => a.SecurityRequirement),
                            Animals = new List<Animal>()
                        };

                        foreach (var animal in subGroup)
                            newEnclosure.Animals.Add(animal);

                        _context.Enclosures.Add(newEnclosure);
                        existingEnclosures.Add(newEnclosure);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
