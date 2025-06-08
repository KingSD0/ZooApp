using System.ComponentModel.DataAnnotations;

namespace ZooApp.Models
{
    /// <summary>
    /// Vertegenwoordigt een verblijf waarin één of meerdere dieren worden ondergebracht.
    /// Elk verblijf heeft eigenschappen zoals klimaat, habitat en beveiligingsniveau.
    /// </summary>
    public class Enclosure
    {
        /// <summary>
        /// Unieke ID van het verblijf (primaire sleutel).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Naam van het verblijf (verplicht).
        /// Bijvoorbeeld: 'Savanne 1', 'Reptielenhuis'.
        /// </summary>
        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Alle dieren die zich momenteel in dit verblijf bevinden.
        /// </summary>
        public ICollection<Animal> Animals { get; set; } = new List<Animal>();

        /// <summary>
        /// Klimaattype van het verblijf, zoals Tropisch, Gematigd of Arctisch.
        /// </summary>
        [Required(ErrorMessage = "Klimaat is verplicht.")]
        public Climate Climate { get; set; }

        /// <summary>
        /// Habitattypes binnen dit verblijf, zoals Bos, Water, of Grasland (flags enum).
        /// </summary>
        [Required(ErrorMessage = "HabitatType is verplicht.")]
        public HabitatType HabitatType { get; set; }

        /// <summary>
        /// Beveiligingsniveau dat vereist is om dit verblijf veilig te houden.
        /// Bijvoorbeeld: Laag, Middel, Hoog.
        /// </summary>
        [Required(ErrorMessage = "Beveiligingsniveau is verplicht.")]
        public SecurityLevel SecurityLevel { get; set; }

        /// <summary>
        /// Totale oppervlakte van het verblijf in vierkante meters.
        /// Moet liggen tussen 0.1 en 10000 m².
        /// </summary>
        [Range(0.1, 10000.0, ErrorMessage = "Oppervlakte moet tussen 0.1 en 10000 m² zijn.")]
        [Display(Name = "Oppervlakte (m²)")]
        public double Size { get; set; }

        /// <summary>
        /// Controleert of het verblijf voldoet aan algemene eisen, zoals minimaal aantal dieren en totale ruimtegebruik.
        /// </summary>
        /// <returns>Een string met statusinformatie of waarschuwingen over het verblijf zelf.</returns>
        public string GetConstraintStatus()
        {
            var messages = new List<string>();

            if (Animals == null || !Animals.Any())
            {
                messages.Add("⚠️ Geen dieren in dit verblijf.");
            }

            double totalSpace = Animals?.Sum(a => a.SpaceRequirement) ?? 0;
            if (Size < totalSpace)
            {
                messages.Add($"⚠️ Onvoldoende ruimte (vereist: {totalSpace:F2} m², beschikbaar: {Size:F2} m²).");
            }

            if (!messages.Any())
                return "✅ Verblijf voldoet aan alle eisen.";

            return string.Join(" ", messages);
        }

    }
}
