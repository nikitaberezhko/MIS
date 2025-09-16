using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIS.Api.Domain.PatientAggregate;

namespace MIS.Api.Infrastructure.Configurations.PatientAggregate;

public class BloodTypeConfiguration : IEntityTypeConfiguration<BloodType>
{
    public void Configure(EntityTypeBuilder<BloodType> builder)
    {
        builder.ToTable("blood_type");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        
        builder.Property(e => e.Value)
            .HasColumnName("value")
            .HasMaxLength(10)
            .IsRequired();
        
        builder.HasIndex(e => e.Value)
            .IsUnique();
        
        builder.ToTable("blood_types");
        
        builder.HasData(BloodType.GetBloodTypes());
    }
}