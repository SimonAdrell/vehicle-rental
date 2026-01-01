using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRental.Data.Enties;

namespace VehicleRental.Data.EntityTypeConfigurations;

public class BookingEntityConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.StartDate).IsRequired();
        builder.Property(b => b.EndDate).IsRequired();
        builder.Property(b => b.DateOfBooking).IsRequired();

        builder.Property(e => e.Id).HasConversion(ValueConverters.BookingIdConverter);
        builder.Property(e => e.ClientId).HasConversion(ValueConverters.ClientIdConverter);
        builder.Property(e => e.VehicleId).HasConversion(ValueConverters.VehicleIdConverter);
    }
}
