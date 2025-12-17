namespace VehicleRental.Api.Models;

public record class VehicleCreateDto
{
    public required string RegistrationNumber { get; set; }
    public long Milage { get; set; }
    public Guid VehicleTypeId { get; set; }
}
