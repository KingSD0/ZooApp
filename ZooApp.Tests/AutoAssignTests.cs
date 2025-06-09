using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ZooApp.Controllers;
using ZooApp.Data;
using ZooApp.Models;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ZooApp.Tests
{
    public class AutoAssignTests
    {
        private ZooContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: "TestZooDb_" + System.Guid.NewGuid()) // Unieke DB per test
                .Options;

            var context = new ZooContext(options);

            // Voeg testdata toe (volledig ingevuld, geen nulls)
            context.Animals.AddRange(
                new Animal
                {
                    Name = "Lion",
                    Size = Size.VeryLarge,
                    DietaryClass = DietaryClass.Carnivore,
                    ActivityPattern = ActivityPattern.Nocturnal,
                    SpaceRequirement = 10,
                    SecurityRequirement = SecurityLevel.High,
                    Prey = new List<Animal>()
                },
                new Animal
                {
                    Name = "Cow",
                    Size = Size.Medium,
                    DietaryClass = DietaryClass.Herbivore,
                    ActivityPattern = ActivityPattern.Cathemeral,
                    SpaceRequirement = 5,
                    SecurityRequirement = SecurityLevel.Low,
                    Prey = new List<Animal>()
                },
                new Animal
                {
                    Name = "Shark",
                    Size = Size.Large,
                    DietaryClass = DietaryClass.Piscivore,
                    ActivityPattern = ActivityPattern.Diurnal,
                    SpaceRequirement = 8,
                    SecurityRequirement = SecurityLevel.Medium,
                    Prey = new List<Animal>()
                }
            );

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void AutoAssign_NieuwStrategy_AssignsAnimalsAndCreatesEnclosures()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new ZooController(context);
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = controller.AutoAssign("nieuw");

            // Assert
            var animals = context.Animals.Include(a => a.Enclosure).ToList();
            var enclosures = context.Enclosures.Include(e => e.Animals).ToList();

            Assert.All(animals, a => Assert.NotNull(a.Enclosure));           // Elk dier moet een verblijf hebben
            Assert.True(enclosures.Count > 0);                               // Er moet minimaal 1 verblijf zijn aangemaakt
            Assert.All(enclosures, e => Assert.True(e.Animals.Any()));      // Elk verblijf moet minstens 1 dier bevatten
            Assert.IsType<RedirectToActionResult>(result);                  // Moet redirecten naar Index
        }
        [Fact]
        public void AutoAssign_AanvullenStrategy_FillsExistingEnclosure()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: "TestZooDb_" + System.Guid.NewGuid())
                .Options;

            using var context = new ZooContext(options);

            // Bestaand verblijf dat deels gevuld is
            var enclosure = new Enclosure
            {
                Name = "Forest - Arctic 1",
                Climate = Climate.Arctic,
                HabitatType = HabitatType.Forest,
                Size = 50,
                SecurityLevel = SecurityLevel.High,
                Animals = new List<Animal>()
            };

            context.Enclosures.Add(enclosure);

            // Dier zonder verblijf dat erbij moet passen
            context.Animals.Add(new Animal
            {
                Name = "Wolf",
                Size = Size.Small,
                DietaryClass = DietaryClass.Carnivore,
                ActivityPattern = ActivityPattern.Nocturnal,
                SpaceRequirement = 5,
                SecurityRequirement = SecurityLevel.High,
                Prey = new List<Animal>(),
                Enclosure = null // expliciet los
            });

            context.SaveChanges();

            var controller = new ZooController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.AutoAssign("aanvullen");

            // Assert
            var updatedEnclosure = context.Enclosures.Include(e => e.Animals).First();
            Assert.Contains(updatedEnclosure.Animals, a => a.Name == "Wolf"); // Wolf moet toegevoegd zijn aan bestaand verblijf
            Assert.Equal(1, updatedEnclosure.Animals.Count);                  // Alleen Wolf in test
            Assert.IsType<RedirectToActionResult>(result);
        }

    }
}
