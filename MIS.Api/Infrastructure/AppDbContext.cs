using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.Entities;

namespace MIS.Api.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Disease> Diseases => Set<Disease>();
    public DbSet<PatientDisease> PatientDiseases => Set<PatientDisease>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MedicalRecordNumber).IsUnique();
            entity.Property(e => e.LastName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.MiddleName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.AddressLine).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(200);
            entity.Property(e => e.Region).HasMaxLength(200);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.InsurancePolicyNumber).HasMaxLength(100);

            entity.HasOne(e => e.PrimaryDoctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(e => e.PrimaryDoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LastName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.MiddleName).HasMaxLength(200);
            entity.Property(e => e.LicenseNumber).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<PatientDisease>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.DiseaseId, e.DiagnosedOn });
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Diagnoses)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Disease)
                .WithMany(d => d.Patients)
                .HasForeignKey(e => e.DiseaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}


