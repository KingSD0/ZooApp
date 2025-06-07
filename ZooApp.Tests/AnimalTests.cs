using Xunit;
using ZooApp.Models;

namespace ZooApp.Tests
{
    /// <summary>
    /// Bevat unittests voor logica binnen het Animal-model, inclusief status bij zonsopkomst, zonsondergang feeding time en constraints.
    /// </summary>
    public class AnimalTests
    {
        /// <summary>
        /// Test of een dier met activiteitspatroon Diurnal 'Wordt wakker' retourneert bij zonsopkomst.
        /// </summary>
        [Fact]
        public void GetSunriseStatus_ReturnsWordtWakker_ForDiurnal()
        {
            // Arrange
            var animal = new Animal { ActivityPattern = ActivityPattern.Diurnal };

            // Act
            var result = animal.GetSunriseStatus();

            // Assert
            Assert.Equal("Wordt wakker", result);
        }

        /// <summary>
        /// Test of een dier met activiteitspatroon Nocturnal 'Gaat slapen' retourneert bij zonsopkomst.
        /// </summary>
        [Fact]
        public void GetSunriseStatus_ReturnsGaatSlapen_ForNocturnal()
        {
            var animal = new Animal { ActivityPattern = ActivityPattern.Nocturnal };

            var result = animal.GetSunriseStatus();

            Assert.Equal("Gaat slapen", result);
        }

        /// <summary>
        /// Test of een dier met activiteitspatroon Cathemeral 'Altijd actief' retourneert bij zonsopkomst.
        /// </summary>
        [Fact]
        public void GetSunriseStatus_ReturnsAltijdActief_ForCathemeral()
        {
            var animal = new Animal { ActivityPattern = ActivityPattern.Cathemeral };

            var result = animal.GetSunriseStatus();

            Assert.Equal("Altijd actief", result);
        }

        /// <summary>
        /// Test of een dier met een ongeldige enumwaarde 'Onbekend' retourneert.
        /// </summary>
        [Fact]
        public void GetSunriseStatus_ReturnsOnbekend_ForInvalidEnum()
        {
            var animal = new Animal { ActivityPattern = (ActivityPattern)999 };

            var result = animal.GetSunriseStatus();

            Assert.Equal("Onbekend", result);
        }

        /// <summary>
        /// Test of de juiste status wordt teruggegeven bij zonsondergang,
        /// afhankelijk van het activiteitspatroon van het dier.
        /// </summary>
        [Fact]
        public void GetSunsetStatus_ReturnsExpectedValues()
        {
            var diurnal = new Animal { ActivityPattern = ActivityPattern.Diurnal };
            var nocturnal = new Animal { ActivityPattern = ActivityPattern.Nocturnal };
            var cathemeral = new Animal { ActivityPattern = ActivityPattern.Cathemeral };

            Assert.Equal("Gaat slapen", diurnal.GetSunsetStatus());
            Assert.Equal("Wordt wakker", nocturnal.GetSunsetStatus());
            Assert.Equal("Altijd actief", cathemeral.GetSunsetStatus());
        }

        /// <summary>
        /// Test of de juiste beschrijving wordt gegenereerd voor FeedingTime,
        /// inclusief het vermelden van prooidieren indien aanwezig.
        /// </summary>
        [Fact]
        public void GetFeedingDescription_ReturnsCorrectText_ForCarnivoreWithPrey()
        {
            // Arrange
            var prey1 = new Animal { Name = "Hertje" };
            var prey2 = new Animal { Name = "Konijn" };
            var predator = new Animal
            {
                Name = "Leeuw",
                DietaryClass = DietaryClass.Carnivore,
                Prey = new List<Animal> { prey1, prey2 }
            };

            // Act
            var result = predator.GetFeedingDescription();

            // Assert
            Assert.Equal("Eet prooidieren: Hertje, Konijn", result);
        }

        /// <summary>
        /// Test of een omnivoor de juiste voedingsomschrijving geeft.
        /// </summary>
        [Fact]
        public void GetFeedingDescription_ReturnsOmnivoreText()
        {
            var omnivore = new Animal { DietaryClass = DietaryClass.Omnivore };
            var result = omnivore.GetFeedingDescription();
            Assert.Equal("Eet zowel planten als vlees", result);
        }

        /// <summary>
        /// Test of GetConstraintStatus een waarschuwing retourneert bij te lage beveiliging.
        /// </summary>
        [Fact]
        public void GetConstraintStatus_ReturnsWarning_WhenSecurityIsTooLow()
        {
            // Arrange
            var animal = new Animal
            {
                Name = "Testleeuw",
                SpaceRequirement = 10,
                SecurityRequirement = SecurityLevel.High
            };

            var enclosure = new Enclosure
            {
                Name = "Testverblijf",
                Size = 15,
                SecurityLevel = SecurityLevel.Medium
            };

            // Act
            var result = animal.GetConstraintStatus(enclosure);

            // Assert
            Assert.Contains("Onvoldoende beveiliging", result);
        }
        
        /// <summary>
        /// Test of GetConstraintStatus een succesmelding retourneert als aan alle eisen is voldaan.
        /// </summary>
        [Fact]
        public void GetConstraintStatus_ReturnsSuccess_WhenAllRequirementsMet()
        {
            // Arrange
            var animal = new Animal
            {
                Name = "Testolifant",
                SpaceRequirement = 20,
                SecurityRequirement = SecurityLevel.Medium
            };

            var enclosure = new Enclosure
            {
                Name = "Groot verblijf",
                Size = 50,
                SecurityLevel = SecurityLevel.High
            };

            // Act
            var result = animal.GetConstraintStatus(enclosure);

            // Assert
            Assert.Equal("✅ Voldoet aan alle verblijfseisen.", result);
        }
    
}
}
