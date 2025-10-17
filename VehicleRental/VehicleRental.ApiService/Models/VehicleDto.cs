namespace VehicleRental.Api.Models;

public record class VehicleDto : DtoBase
{
    public required string RegistrationNumber { get; set; }
    public long Milage { get; set; }
    public bool IsRemoved { get; set; }
    public required int TypeOfVehicleId { get; set; }

}
