using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using OVOVAX.Core.Entities;
using OVOVAX.Core.Entities.Scanner;
using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Entities.ManualControl;

namespace OVOVAX.Repository.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<ScanResult> ScanResults { get; set; }
        public DbSet<InjectionOperation> InjectionOperations { get; set; }
        public DbSet<MovementCommand> MovementCommands { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ScanResult - SensorReadings will be stored as comma-separated string
            modelBuilder.Entity<ScanResult>()
                .Property(s => s.SensorReadings)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(double.Parse).ToList()
                )
                .Metadata.SetValueComparer(
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<double>>(
                        (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                        c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c == null ? new List<double>() : c.ToList()
                    )
                );

            modelBuilder.Entity<InjectionOperation>()
                .Property(io => io.RangeOfInfraredFrom)
                .HasPrecision(18, 3);
                
            modelBuilder.Entity<InjectionOperation>()
                .Property(io => io.RangeOfInfraredTo)
                .HasPrecision(18, 3);

            modelBuilder.Entity<InjectionOperation>()
                .Property(io => io.StepOfInjection)
                .HasPrecision(18, 3);

            modelBuilder.Entity<InjectionOperation>()
                .Property(io => io.VolumeOfLiquid)
                .HasPrecision(18, 3);        }
    }
}
