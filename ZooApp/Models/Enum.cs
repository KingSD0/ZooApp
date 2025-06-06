namespace ZooApp.Models
{
    // Grootteklasse voor dieren, gebruikt voor ruimteberekening
    public enum Size { Microscopic, VerySmall, Small, Medium, Large, VeryLarge }

    // Voedingsklassen voor dieren
    public enum DietaryClass { Carnivore, Herbivore, Omnivore, Insectivore, Piscivore }

    // Dag-/nachtritme van dieren
    public enum ActivityPattern { Diurnal, Nocturnal, Cathemeral }

    // Beveiligingsniveau dat nodig is voor een dier of verblijf
    public enum SecurityLevel { Low, Medium, High }

    // Type leefomgeving per verblijf (Flags voor combinaties)
    [Flags]
    public enum HabitatType
    {
        None = 0,
        Forest = 1,
        Aquatic = 2,
        Desert = 4,
        Grassland = 8
    }

    // Klimaattype van een verblijf
    public enum Climate { Tropical, Temperate, Arctic }
}
