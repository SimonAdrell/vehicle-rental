namespace VehicleRental.Data.Enties;

public class VehicleTypeEntity
{
    public VehicleTypeId Id { get; set; } = VehicleTypeId.Empty;
    public required string Name { get; set; }
    public string? Description { get; set; }
    public double PricePerDay { get; set; }
    public double? DayMultiplier { get; set; }
    public double? PricePerKilometer { get; set; }
    public double? KilometerMultiplier { get; set; }
    public DateTime? DateOfDeletion { get; set; }
    public bool IsDeleted { get; set; }
}

public record struct VehicleTypeId(Guid Id)
{
    public static VehicleTypeId Empty => new(Guid.Empty);
    public static VehicleTypeId NewVehicleTypeId() => new(Guid.NewGuid());
}
