namespace VehicleRental.Data.Enties;

public record class VehicleTypeEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public double PricePerDay { get; set; }
    public double? DayMultiplier { get; set; }
    public double? PricePerKilometer { get; set; }
    public double? KilometerMultiplier { get; set; }
    public DateTime? DateOfDeletion { get; set; }
    public bool IsDeleted { get; set; }
}
