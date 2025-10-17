namespace VehicleRental.Api.Models;

public record class Booking : DtoBase
{
    public required int VehicleId { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
}
