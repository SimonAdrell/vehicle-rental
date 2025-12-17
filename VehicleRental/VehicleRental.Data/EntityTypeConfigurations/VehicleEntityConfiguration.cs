using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRental.Data.Enties;

namespace VehicleRental.Data.EntityTypeConfigurations;

public class VehicleEntityConfiguration : IEntityTypeConfiguration<VehicleEntity>
{
    public void Configure(EntityTypeBuilder<VehicleEntity> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.RegistrationNumber).IsRequired().HasMaxLength(20);
        builder.Property(v => v.Milage).IsRequired();
        builder.Property(v => v.IsRemoved).IsRequired().HasDefaultValue(false);
        builder.Property(v => v.Id).HasConversion(ValueConverters.VehicleIdConverter);
        builder.Property(v => v.TypeOfVehicleId).HasConversion(ValueConverters.VehicleTypeIdConverter);
        // Unique constraint: Only one active vehicle per registration number
        builder.HasIndex(v => v.RegistrationNumber)
              .IsUnique()
              .HasFilter("[IsRemoved] = 0");

        // Relationship with TypeOfVehicle
        builder.HasOne(v => v.TypeOfVehicle)
              .WithMany()
              .HasForeignKey(v => v.TypeOfVehicleId)
              .IsRequired();

        // Relationship with Bookings
        builder.HasMany<BookingEntity>(v => v.Bookings)
              .WithOne(b => b.Vehicle)
              .HasForeignKey(b => b.VehicleId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
