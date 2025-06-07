namespace ZooApp.Models
{
    /// <summary>
    /// Grootteklasse voor dieren, gebruikt o.a. bij ruimteplanning en constraint-checks.
    /// </summary>
    public enum Size
    {
        Microscopic,    // < 1 cm
        VerySmall,      // 1–10 cm
        Small,          // 10–50 cm
        Medium,         // 50–150 cm
        Large,          // 150–300 cm
        VeryLarge       // > 300 cm
    }

    /// <summary>
    /// Voedingsklassen die aangeven wat een dier eet.
    /// </summary>
    public enum DietaryClass
    {
        Carnivore,      // Vleeseter
        Herbivore,      // Planteneter
        Omnivore,       // Alleseter
        Insectivore,    // Insecteneter
        Piscivore       // Viseter
    }

    /// <summary>
    /// Activiteitspatroon van dieren, beïnvloedt Sunrise/Sunset gedrag.
    /// </summary>
    public enum ActivityPattern
    {
        Diurnal,        // Actief overdag
        Nocturnal,      // Actief 's nachts
        Cathemeral      // Verspreid over dag en nacht
    }

    /// <summary>
    /// Beveiligingsniveau dat nodig is voor een dier of verblijf, bijvoorbeeld vanwege gevaarlijk gedrag.
    /// </summary>
    public enum SecurityLevel
    {
        Low,        // Weinig risico
        Medium,     // Gematigd risico
        High        // Hoog risico (bijv. roofdieren)
    }

    /// <summary>
    /// Type leefomgeving binnen een verblijf, combineerbaar via Flags.
    /// </summary>
    [Flags]
    public enum HabitatType
    {
        None = 0,       // Geen specifiek type
        Forest = 1,     // Bosrijk gebied
        Aquatic = 2,    // Wateromgeving
        Desert = 4,     // Woestijnachtig gebied
        Grassland = 8   // Grasvlakten of savanne
    }

    /// <summary>
    /// Klimaatzone waarin een verblijf valt.
    /// </summary>
    public enum Climate
    {
        Tropical,   // Warm en vochtig
        Temperate,  // Gematigd klimaat
        Arctic      // Koud en ijzig
    }
}
