using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRental.Data.Enties;

namespace VehicleRental.Data.EntityTypeConfigurations;

public class ClientEntityConfiguration : IEntityTypeConfiguration<ClientEntity>
{
    public void Configure(EntityTypeBuilder<ClientEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.IdentificationNumber).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Name).HasMaxLength(100);
        builder.Property(c => c.Email).HasMaxLength(255);
        builder.Property(c => c.PhoneNumber).HasMaxLength(20);
        builder.Property(c => c.Id).HasConversion(ValueConverters.ClientIdConverter);

        // Unique constraint for identification number
        builder.HasIndex(c => c.IdentificationNumber)
              .IsUnique();

        // Relationship with Bookings
        builder.HasMany<BookingEntity>(c => c.Bookings)
              .WithOne(b => b.Client)
              .HasForeignKey(b => b.ClientId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
