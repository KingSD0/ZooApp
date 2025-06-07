using Xunit;
using ZooApp.Models;

namespace ZooApp.Tests
{
    /// <summary>
    /// Bevat unittests voor de GetSunriseStatus-methode van Animal.
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

    }
}
