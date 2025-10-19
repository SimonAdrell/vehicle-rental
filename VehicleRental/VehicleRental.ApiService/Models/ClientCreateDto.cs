namespace VehicleRental.Api.Models;

public class ClientCreateDto
{
    public required string IdentificationNumber { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
