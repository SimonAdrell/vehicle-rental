namespace VehicleRental.Api.Models;

public record class ClientUpdateDto
{
    public required string IdentificationNumber { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
