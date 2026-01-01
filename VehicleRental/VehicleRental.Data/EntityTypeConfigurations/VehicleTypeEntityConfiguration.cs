using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRental.Data.Enties;

namespace VehicleRental.Data.EntityTypeConfigurations;

public class VehicleTypeEntityConfiguration : IEntityTypeConfiguration<VehicleTypeEntity>
{
    public void Configure(EntityTypeBuilder<VehicleTypeEntity> builder)
    {
        builder.HasKey(t => t.Id)
;
        builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Description).HasMaxLength(200);
        builder.Property(t => t.Id)
            .HasConversion(ValueConverters.VehicleTypeIdConverter)
            .ValueGeneratedOnAdd();

        // Unique constraint for type name
        builder.HasIndex(t => t.Name)
              .IsUnique();
    }
}
