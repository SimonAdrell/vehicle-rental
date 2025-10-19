namespace VehicleRental.Api.Models;

public record class BookingDto : DtoBase
{
    public double? Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required ClientDto Client { get; set; }
    public required VehicleDto Vehicle { get; set; }
    public DateTime DateOfBooking { get; set; }
    public DateTime? DateOfRelease { get; set; }
    public DateTime? DateOfReturn { get; set; }
    public long? StartMilage { get; set; }
    public long? EndMilage { get; set; }
}
