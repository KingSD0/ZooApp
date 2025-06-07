using System.Collections.Generic;
using System.Linq;
using Xunit;
using ZooApp.Models;

namespace ZooApp.Tests
{
    /// <summary>
    /// Bevat tests voor logica die bepaalt of er een waarschuwing nodig is bij Feeding Time.
    /// </summary>
    public class FeedingLogicTests
    {
        /// <summary>
        /// Test of een waarschuwing getoond moet worden wanneer een carnivoor zijn prooi deelt binnen hetzelfde verblijf.
        /// </summary>
        [Fact]
        public void Warning_Shown_When_Carnivore_With_Prey_In_Same_Enclosure()
        {
            // Arrange
            var prey = new Animal { Name = "Hertje" };
            var predator = new Animal
            {
                Name = "Leeuw",
                DietaryClass = DietaryClass.Carnivore,
                Prey = new List<Animal> { prey }
            };

            var enclosure = new Enclosure
            {
                Name = "Savanne",
                Animals = new List<Animal> { predator, prey }
            };

            // Act
            bool warningShown = enclosure.Animals.Any(pred =>
                pred.DietaryClass == DietaryClass.Carnivore &&
                pred.Prey != null &&
                pred.Prey.Any(p => enclosure.Animals.Contains(p)));

            // Assert
            Assert.True(warningShown);
        }

        /// <summary>
        /// Test dat er géén waarschuwing komt als carnivoor geen prooi deelt in hetzelfde verblijf.
        /// </summary>
        [Fact]
        public void NoWarning_When_Prey_NotInSameEnclosure()
        {
            // Arrange
            var prey = new Animal { Name = "Konijn" };
            var predator = new Animal
            {
                Name = "Tijger",
                DietaryClass = DietaryClass.Carnivore,
                Prey = new List<Animal> { prey }
            };

            var enclosure = new Enclosure
            {
                Name = "Regenwoud",
                Animals = new List<Animal> { predator } // Geen konijn in verblijf
            };

            // Act
            bool warningShown = enclosure.Animals.Any(pred =>
                pred.DietaryClass == DietaryClass.Carnivore &&
                pred.Prey != null &&
                pred.Prey.Any(p => enclosure.Animals.Contains(p)));

            // Assert
            Assert.False(warningShown);
        }
    }
}
