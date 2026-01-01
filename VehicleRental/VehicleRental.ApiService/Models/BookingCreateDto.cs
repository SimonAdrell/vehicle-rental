namespace VehicleRental.Api.Models;

public record BookingCreateDto : DtoBase
{
    public Guid ClientId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime DateOfBooking { get; set; } = DateTime.UtcNow;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? DateOfRelease { get; set; }
    public DateTime? DateOfReturn { get; set; }
    public long? StartMilage { get; set; }
    public long? EndMilage { get; set; }
}
