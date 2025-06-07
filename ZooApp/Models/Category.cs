using System.ComponentModel.DataAnnotations;

namespace ZooApp.Models
{
    /// <summary>
    /// Vertegenwoordigt een diercategorie, zoals 'Zoogdieren', 'Reptielen', enz.
    /// Categorieën helpen bij het ordenen van dieren binnen de dierentuin.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Unieke ID van de categorie (primaire sleutel).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Naam van de categorie (bijv. 'Vogels', 'Amfibieën').
        /// Verplicht veld, getoond als 'Categorienaam' in de UI.
        /// </summary>
        [Required(ErrorMessage = "Naam is verplicht.")]
        [Display(Name = "Categorienaam")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Collectie van alle dieren die tot deze categorie behoren.
        /// </summary>
        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
