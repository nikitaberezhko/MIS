using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIS.Api.Domain.DoctorAggregate;

namespace MIS.Api.Infrastructure.Configurations.DoctorAggregate;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctor");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(x => x.Id).HasColumnName("id");
        
        builder.HasOne(x => x.Specialty)
            .WithMany()
            .HasForeignKey("specialty_id");
        
        builder.HasOne(x => x.License)
            .WithMany()
            .HasForeignKey("license_id");

        builder.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
        
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
        
        builder.OwnsOne(e => e.Contacts, contacts =>
        {
            contacts.Property(c => c.Phone)
                .HasColumnName("phone")
                .HasMaxLength(50);
            contacts.Property(c => c.Email)
                .HasColumnName("email")
                .HasMaxLength(320);
        });
    }
}