namespace VehicleRental.Api.Models;

public record class FastBooking
{
    public required string BookingNumber { get; set; }
    public required string RegistrationNumber { get; set; }
    public required string ClientIdentificationNumber { get; set; }
    public required string TypeOfVehicle { get; set; }
    public required DateTime StartDate { get; set; }
    public required long Milage { get; set; }
}
