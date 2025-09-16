using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIS.Api.Domain.DoctorAggregate;

namespace MIS.Api.Infrastructure.Configurations.DoctorAggregate;

public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
{
    public void Configure(EntityTypeBuilder<Specialty> builder)
    {
        builder.ToTable("specialty");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        
        builder.Property(e => e.Value)
            .HasColumnName("value")
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(200)
            .IsRequired();
        
        builder.HasIndex(e => e.Value)
            .IsUnique();
        
        builder.HasIndex(e => e.DisplayName)
            .IsUnique();
        
        builder.HasData(Specialty.AllSpecialties());
    }
}