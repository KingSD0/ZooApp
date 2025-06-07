using System.ComponentModel.DataAnnotations;

namespace ZooApp.Models
{
    /// <summary>
    /// Vertegenwoordigt een dier in de dierentuin en bevat informatie over o.a. soort, grootte, activiteit en verblijf.
    /// </summary>
    public class Animal
    {
        /// <summary>
        /// Unieke ID van het dier (primaire sleutel).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Naam van het dier (verplicht, max. 50 tekens).
        /// </summary>
        [Required(ErrorMessage = "Naam is verplicht.")]
        [StringLength(50, ErrorMessage = "Naam mag maximaal 50 tekens bevatten.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Soortnaam of wetenschappelijke benaming van het dier (verplicht, max. 50 tekens).
        /// </summary>
        [Required(ErrorMessage = "Soort is verplicht.")]
        [StringLength(50, ErrorMessage = "Soort mag maximaal 50 tekens bevatten.")]
        public string Species { get; set; } = string.Empty;

        /// <summary>
        /// Optionele koppeling naar een diercategorie (bijv. roofdieren, herbivoren).
        /// </summary>
        [Display(Name = "Categorie")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        /// <summary>
        /// Grootte van het dier (verplicht) op basis van vooraf gedefinieerde Size-enum.
        /// </summary>
        [Required(ErrorMessage = "Grootte is verplicht.")]
        [Display(Name = "Grootte")]
        public Size Size { get; set; }

        /// <summary>
        /// Voedingsgedrag van het dier (verplicht) op basis van DietaryClass-enum.
        /// </summary>
        [Required(ErrorMessage = "Voedingsklasse is verplicht.")]
        [Display(Name = "Voedingsklasse")]
        public DietaryClass DietaryClass { get; set; }

        /// <summary>
        /// Activiteitspatroon van het dier (verplicht), zoals Diurnal of Nocturnal.
        /// </summary>
        [Required(ErrorMessage = "Activiteitspatroon is verplicht.")]
        [Display(Name = "Activiteitspatroon")]
        public ActivityPattern ActivityPattern { get; set; }

        /// <summary>
        /// Optionele lijst van prooidieren die dit dier eet (voor FeedingTime).
        /// </summary>
        public ICollection<Animal>? Prey { get; set; }

        /// <summary>
        /// Verblijf waarin het dier zich bevindt (optioneel).
        /// </summary>
        [Display(Name = "Verblijf")]
        public int? EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }

        /// <summary>
        /// Benodigde ruimte (in m²) voor dit dier, verplicht tussen 0.1 en 1000 m².
        /// </summary>
        [Required(ErrorMessage = "Ruimtevereiste is verplicht.")]
        [Range(0.1, 1000.0, ErrorMessage = "Ruimtevereiste moet tussen 0.1 en 1000 m² liggen.")]
        [Display(Name = "Ruimtevereiste (m²)")]
        public double SpaceRequirement { get; set; }

        /// <summary>
        /// Minimale vereiste beveiliging voor dit dier (verplicht).
        /// </summary>
        [Required(ErrorMessage = "Beveiligingsniveau is verplicht.")]
        [Display(Name = "Beveiligingsniveau")]
        public SecurityLevel SecurityRequirement { get; set; }

        /// <summary>
        /// Bepaalt de status van het dier bij zonsopkomst op basis van het activiteitspatroon.
        /// </summary>
        /// <returns>
        /// "Wordt wakker" voor Diurnal, "Gaat slapen" voor Nocturnal, "Altijd actief" voor Cathemeral, of "Onbekend" voor overige.
        /// </returns>
        public string GetSunriseStatus()
        {
            return ActivityPattern switch
            {
                ActivityPattern.Diurnal => "Wordt wakker",
                ActivityPattern.Nocturnal => "Gaat slapen",
                ActivityPattern.Cathemeral => "Altijd actief",
                _ => "Onbekend"
            };
        }

        public string GetSunsetStatus()
        {
            return ActivityPattern switch
            {
                ActivityPattern.Diurnal => "Gaat slapen",
                ActivityPattern.Nocturnal => "Wordt wakker",
                ActivityPattern.Cathemeral => "Altijd actief",
                _ => "Onbekend"
            };
        }

    }
}
