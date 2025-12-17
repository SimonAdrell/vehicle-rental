namespace VehicleRental.Data.Enties;

public record VehicleEntity
{
    public VehicleId Id { get; set; } = VehicleId.Empty;
    public required string RegistrationNumber { get; set; }
    public long Milage { get; set; }
    public bool IsRemoved { get; set; }
    public VehicleTypeId TypeOfVehicleId { get; set; }
    public required VehicleTypeEntity TypeOfVehicle { get; set; }
    public IEnumerable<BookingEntity>? Bookings { get; set; }
}

public record struct VehicleId(Guid Id)
{
    public static VehicleId Empty => new(Guid.Empty);
    public static VehicleId NewVehicleId() => new(Guid.NewGuid());
}
