using System.ComponentModel.DataAnnotations;

namespace ZooApp.Models
{
    /// <summary>
    /// Vertegenwoordigt een verblijf waarin dieren geplaatst worden.
    /// </summary>
    public class Enclosure
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Animal> Animals { get; set; } = new List<Animal>();

        [Required(ErrorMessage = "Klimaat is verplicht.")]
        public Climate Climate { get; set; }

        [Required(ErrorMessage = "HabitatType is verplicht.")]
        public HabitatType HabitatType { get; set; }

        [Required(ErrorMessage = "Beveiligingsniveau is verplicht.")]
        public SecurityLevel SecurityLevel { get; set; }

        [Range(0.1, 10000.0, ErrorMessage = "Oppervlakte moet tussen 0.1 en 10000 m² zijn.")]
        [Display(Name = "Oppervlakte (m²)")]
        public double Size { get; set; }
    }
}
