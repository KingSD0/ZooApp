namespace ZooApp.Models
{
    /// <summary>
    /// Vertegenwoordigt een verblijf waarin dieren geplaatst worden.
    /// </summary>
    public class Enclosure
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // Naam van het verblijf

        public ICollection<Animal> Animals { get; set; } = new List<Animal>(); // Dieren in dit verblijf

        public Climate Climate { get; set; } // Klimaattype (enum)

        public HabitatType HabitatType { get; set; } // Leefomgeving (Flags enum)

        public SecurityLevel SecurityLevel { get; set; } // Beveiligingsniveau (enum)

        public double Size { get; set; } // Totale ruimte in m²
    }
}
