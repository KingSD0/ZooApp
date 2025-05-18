namespace ZooApp.Models
{
    /// <summary>
    /// Vertegenwoordigt een categorie zoals 'Zoogdieren', 'Reptielen', etc.
    /// </summary>
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // Naam van de categorie

        public ICollection<Animal> Animals { get; set; } = new List<Animal>(); // Alle dieren in deze categorie
    }
}
