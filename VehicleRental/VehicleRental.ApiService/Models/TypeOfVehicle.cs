namespace VehicleRental.Api.Models;

public record class TypeOfVehicle : DtoBase
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public double PricePerDay { get; set; }
    public double? DayMultiplier { get; set; }
    public double PricePerKilometer { get; set; }
    public double? KilometerMultiplier { get; set; }

}
