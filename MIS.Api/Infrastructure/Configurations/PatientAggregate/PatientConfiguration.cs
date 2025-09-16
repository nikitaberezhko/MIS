using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIS.Api.Domain.DoctorAggregate;
using MIS.Api.Domain.PatientAggregate;

namespace MIS.Api.Infrastructure.Configurations.PatientAggregate;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patient");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        
        builder.HasOne<Doctor>()
            .WithMany()
            .HasForeignKey(e => e.PrimaryDoctorId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
        
        builder.Property(x => x.PrimaryDoctorId).HasColumnName("primary_doctor_id");
        
        builder.HasOne(x => x.BloodType)
            .WithMany().HasForeignKey("blood_type_id").HasConstraintName("FK_blood_type_id").IsRequired();
        
        builder.OwnsOne(e => e.MedicalRecordNumber, mr =>
        {
            mr.Property(m => m.Value)
                .HasColumnName("medical_record_number")
                .HasMaxLength(50)
                .IsRequired();
            mr.HasIndex(m => m.Value).IsUnique();
        });
        
        builder.OwnsOne(e => e.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(200)
                .IsRequired();
            name.Property(n => n.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(200)
                .IsRequired();
            name.Property(n => n.MiddleName)
                .HasColumnName("middle_name")
                .HasMaxLength(200);
        });
        
        builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.AddressLine)
                .HasColumnName("address_line")
                .HasMaxLength(500)
                .IsRequired();
            address.Property(a => a.City)
                .HasColumnName("city")
                .HasMaxLength(200)
                .IsRequired();
            address.Property(a => a.Region)
                .HasColumnName("region")
                .HasMaxLength(200)
                .IsRequired();
            address.Property(a => a.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(20)
                .IsRequired();
        });
        
        builder.OwnsOne(e => e.Contacts, contacts =>
        {
            contacts.Property(c => c.Phone)
                .HasColumnName("phone")
                .HasMaxLength(50);
            contacts.Property(c => c.Email)
                .HasColumnName("email")
                .HasMaxLength(320);
        });
        
        builder.OwnsOne(e => e.Sex, sex =>
        {
            sex.Property(s => s.Value)
                .HasColumnName("sex")
                .HasMaxLength(20)
                .IsRequired();
        });
    }
}