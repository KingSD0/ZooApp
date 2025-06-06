using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using ZooApp.Models;

namespace ZooApp.Data
{
    /// <summary>
    /// De databasecontext voor de dierentuinapplicatie.
    /// </summary>
    public class ZooContext : DbContext
    {
        public ZooContext(DbContextOptions<ZooContext> options) : base(options)
        {
        }

        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enclosure> Enclosures => Set<Enclosure>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuratie voor self-referencing relatie (Prey)
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.Prey)
                .WithMany();

            // Enum met Flags (HabitatType) wordt als int opgeslagen
            modelBuilder.Entity<Enclosure>()
                .Property(e => e.HabitatType)
                .HasConversion<int>();
        }
    }
}
