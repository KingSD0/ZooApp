using System.ComponentModel.DataAnnotations;

namespace ZooApp.Models
{
    /// <summary>
    /// Vertegenwoordigt een dier in de dierentuin.
    /// </summary>
    public class Animal
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // Naam van het dier

        public string Species { get; set; } = string.Empty; // Soortnaam (bv. Panthera leo)

        public int? CategoryId { get; set; } // FK naar categorie (optioneel)
        public Category? Category { get; set; }

        public Size Size { get; set; } // Grootteklasse (enum)

        public DietaryClass DietaryClass { get; set; } // Voedingsklasse (enum)

        public ActivityPattern ActivityPattern { get; set; } // Activiteitspatroon (enum)

        public ICollection<Animal>? Prey { get; set; } // Andere dieren die dit dier eet

        public int? EnclosureId { get; set; } // FK naar verblijf (optioneel)
        public Enclosure? Enclosure { get; set; }

        public double SpaceRequirement { get; set; } // Vereiste ruimte in m² per dier

        public SecurityLevel SecurityRequirement { get; set; } // Nodig beveiligingsniveau (enum)
    }
}
