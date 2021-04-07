using PetClinic.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace PetClinic.Data
{

    public class PetClinicContext : DbContext
    {
        public PetClinicContext()
        {
        }

        public PetClinicContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<AnimalAid> AnimalAids { get; set; }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<ProcedureAnimalAid> ProceduresAnimalAids { get; set; }
        public DbSet<Vet> Vets { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProcedureAnimalAid>(entity =>
            {
                entity.HasKey(paa => new { paa.ProcedureId, paa.AnimalAidId });

                entity.HasOne(paa => paa.Procedure)
                .WithMany(p => p.ProcedureAnimalAids)
                .HasForeignKey(paa => paa.ProcedureId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(paa => paa.AnimalAid)
                .WithMany(aa => aa.AnimalAidProcedures)
                .HasForeignKey(paa => paa.AnimalAidId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Procedure>(entity =>
            {
                entity.HasOne(p => p.Vet)
                .WithMany(v => v.Procedures)
                .HasForeignKey(p => p.VetId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Animal)
                .WithMany(a => a.Procedures)
                .HasForeignKey(p => p.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //builder.Entity<Animal>(entity =>
            //{
            //    entity.HasOne(a => a.Passport)
            //    .WithOne(p => p.Animal)
            //    .OnDelete(DeleteBehavior.Restrict);
            //});

            //builder.Entity<AnimalAid>(entity =>
            //{
            //    entity.HasAlternateKey(aa=>aa.Name);
            //});

            //builder.Entity<Vet>(entity =>
            //{
            //    entity.HasAlternateKey(v=>v.PhoneNumber);
            //});
        }
    }
}