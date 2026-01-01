using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VehicleRental.Data.Enties;

namespace VehicleRental.Data.EntityTypeConfigurations;

public static class ValueConverters
{
    public readonly static ValueConverter<BookingId, Guid> BookingIdConverter = new(
          v => v.Id,
          v => new BookingId(v));

    public readonly static ValueConverter<ClientId, Guid> ClientIdConverter = new(
      v => v.Value,
      v => new ClientId(v)
    );

    public readonly static ValueConverter<VehicleId, Guid> VehicleIdConverter = new(
      v => v.Id,
      v => new VehicleId(v)
    );

    public readonly static ValueConverter<VehicleTypeId, Guid> VehicleTypeIdConverter = new(
      v => v.Id,
      v => new VehicleTypeId(v)
    );
}
