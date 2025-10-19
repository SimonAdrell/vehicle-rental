namespace VehicleRental.Api.Models;

public record class BookingReleaseDto
{
    public long CurrentMilage { get; set; }
    public DateTime ReleaseDate { get; set; }
}
