namespace VehicleRental.Api.Models;

public record class VehicleDto : DtoBase
{
    public required string RegistrationNumber { get; set; }
    public long Milage { get; set; }
    public bool IsRemoved { get; set; }
    public int? VehicleTypeId { get; set; }
    public VehicleTypeDto? VehicleType { get; set; }

}
