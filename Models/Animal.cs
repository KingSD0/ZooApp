using System.ComponentModel.DataAnnotations;

namespace ZooApp.Models
{
    /// <summary>
    /// Vertegenwoordigt een dier in de dierentuin.
    /// </summary>
    public class Animal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naam is verplicht.")]
        [StringLength(50, ErrorMessage = "Naam mag maximaal 50 tekens bevatten.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soort is verplicht.")]
        [StringLength(50, ErrorMessage = "Soort mag maximaal 50 tekens bevatten.")]
        public string Species { get; set; } = string.Empty;

        [Display(Name = "Categorie")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Grootte is verplicht.")]
        [Display(Name = "Grootte")]
        public Size Size { get; set; }

        [Required(ErrorMessage = "Voedingsklasse is verplicht.")]
        [Display(Name = "Voedingsklasse")]
        public DietaryClass DietaryClass { get; set; }

        [Required(ErrorMessage = "Activiteitspatroon is verplicht.")]
        [Display(Name = "Activiteitspatroon")]
        public ActivityPattern ActivityPattern { get; set; }

        public ICollection<Animal>? Prey { get; set; }

        [Display(Name = "Verblijf")]
        public int? EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }

        [Required(ErrorMessage = "Ruimtevereiste is verplicht.")]
        [Range(0.1, 1000.0, ErrorMessage = "Ruimtevereiste moet tussen 0.1 en 1000 m² liggen.")]
        [Display(Name = "Ruimtevereiste (m²)")]
        public double SpaceRequirement { get; set; }

        [Required(ErrorMessage = "Beveiligingsniveau is verplicht.")]
        [Display(Name = "Beveiligingsniveau")]
        public SecurityLevel SecurityRequirement { get; set; }
    }
}
