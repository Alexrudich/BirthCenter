using Microsoft.EntityFrameworkCore;
using BirthCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BirthCenter.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Family)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Gender)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(p => p.BirthDate)
                    .IsRequired();

                entity.Property(p => p.Use)
                    .HasMaxLength(50);

                entity.Property(p => p.Given)
                    .HasColumnType("jsonb");

                entity.Property(p => p.Active)
                    .HasDefaultValue(true);
            });
        }
    }
}