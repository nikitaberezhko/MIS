using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIS.Api.Domain.DiseaseAggregate;

namespace MIS.Api.Infrastructure.Configurations.DiseaseAggregate;

public class DiseaseConfiguration : IEntityTypeConfiguration<Disease>
{
    public void Configure(EntityTypeBuilder<Disease> builder)
    {
        builder.ToTable("disease");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        
        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(300)
            .IsRequired();
        
        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);
        
        builder.Property(e => e.IsChronic)
            .HasColumnName("is_chronic")
            .IsRequired();
        
        builder.Property(e => e.IsInfectious)
            .HasColumnName("is_infectious")
            .IsRequired();
    }
}