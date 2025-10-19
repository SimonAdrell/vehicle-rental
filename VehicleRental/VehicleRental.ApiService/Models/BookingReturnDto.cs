namespace VehicleRental.Api.Models;

public record class BookingReturnDto
{
    public long Milage { get; set; }
    public DateTime DateOfReturn { get; set; }
}
