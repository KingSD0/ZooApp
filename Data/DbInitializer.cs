using Bogus;
using Microsoft.EntityFrameworkCore;
using ZooApp.Models;

namespace ZooApp.Data
{
    public static class DbInitializer
    {
        public static void Seed(ZooContext context)
        {
            context.Database.Migrate();

            if (context.Categories.Any() || context.Enclosures.Any() || context.Animals.Any())
                return; // Database is al gevuld

            var categories = new List<Category>
            {
                new Category { Name = "Zoogdieren" },
                new Category { Name = "Reptielen" },
                new Category { Name = "Vogels" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            var enclosures = new List<Enclosure>
            {
                new Enclosure { Name = "Savanne", Climate = Climate.Temperate, HabitatType = HabitatType.Grassland | HabitatType.Forest, SecurityLevel = SecurityLevel.Medium, Size = 300 },
                new Enclosure { Name = "Regenwoud", Climate = Climate.Tropical, HabitatType = HabitatType.Forest, SecurityLevel = SecurityLevel.High, Size = 500 },
                new Enclosure { Name = "Poolgebied", Climate = Climate.Arctic, HabitatType = HabitatType.Grassland, SecurityLevel = SecurityLevel.High, Size = 400 }
            };
            context.Enclosures.AddRange(enclosures);
            context.SaveChanges();

            var faker = new Faker("nl");
            var animalFaker = new Faker<Animal>("nl")
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Species, f => f.PickRandom(new[] { "Leeuw", "Tijger", "Olifant", "Krokodil", "Papegaai" }))
                .RuleFor(a => a.Size, f => f.PickRandom<Size>())
                .RuleFor(a => a.DietaryClass, f => f.PickRandom<DietaryClass>())
                .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
                .RuleFor(a => a.SpaceRequirement, f => Math.Round(f.Random.Double(1, 20), 2))
                .RuleFor(a => a.SecurityRequirement, f => f.PickRandom<SecurityLevel>())
                .RuleFor(a => a.Category, f => f.PickRandom(categories))
                .RuleFor(a => a.Enclosure, f => f.PickRandom(enclosures));

            var animals = animalFaker.Generate(10);
            context.Animals.AddRange(animals);
            context.SaveChanges();
        }
    }
}
