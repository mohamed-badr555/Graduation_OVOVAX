using Microsoft.EntityFrameworkCore;
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
        public DbSet<InjectionSession> InjectionSessions { get; set; }
        public DbSet<InjectionRecord> InjectionRecords { get; set; }
        public DbSet<MovementCommand> MovementCommands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<InjectionRecord>()
                .HasOne(ir => ir.InjectionSession)
                .WithMany(iss => iss.InjectionRecords)
                .HasForeignKey(ir => ir.InjectionSessionId);

            // Configure property precision for SQL Server
            modelBuilder.Entity<ScanResult>()
                .Property(s => s.DepthMeasurement)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InjectionSession>()
                .Property(iss => iss.RangeOfInfrared)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InjectionSession>()
                .Property(iss => iss.StepOfInjection)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InjectionSession>()
                .Property(iss => iss.VolumeOfLiquid)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InjectionRecord>()
                .Property(ir => ir.VolumeInjected)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MovementCommand>()
                .Property(mc => mc.Step)
                .HasPrecision(18, 2);
        }
    }
}
