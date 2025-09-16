using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIS.Api.Domain.DoctorAggregate;

namespace MIS.Api.Infrastructure.Configurations.DoctorAggregate;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("license");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        
        builder.Property(e => e.Number)
            .HasColumnName("number")
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.ValidUntil)
            .HasColumnName("valid_until")
            .HasColumnType("date")
            .IsRequired();
        
        builder.HasIndex(e => e.Number)
            .IsUnique();
        
        builder.HasIndex(e => e.ValidUntil);
        
        builder.Property(e => e.ValidUntil)
            .HasConversion(
                dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime));
    }
}