using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.DiseaseAggregate;
using MIS.Api.Domain.DoctorAggregate;
using MIS.Api.Domain.PatientAggregate;
using MIS.Api.Infrastructure.Configurations.DiseaseAggregate;
using MIS.Api.Infrastructure.Configurations.DoctorAggregate;
using MIS.Api.Infrastructure.Configurations.PatientAggregate;

namespace MIS.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors {get; set; }
    public DbSet<Disease> Diseases { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Patient aggregate
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new BloodTypeConfiguration());
        
        // Doctor aggregate
        modelBuilder.ApplyConfiguration(new DoctorConfiguration());
        modelBuilder.ApplyConfiguration(new SpecialtyConfiguration());
        modelBuilder.ApplyConfiguration(new LicenseConfiguration());
        
        // Disease aggregate
        modelBuilder.ApplyConfiguration(new DiseaseConfiguration());
    }
}


