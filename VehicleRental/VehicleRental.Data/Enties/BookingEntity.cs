namespace VehicleRental.Data.Enties;

public record BookingEntity
{
    public BookingId Id { get; set; } = BookingId.Empty;
    public required ClientId ClientId { get; set; }
    public required ClientEntity Client { get; set; }
    public required VehicleId VehicleId { get; set; }
    public required VehicleEntity Vehicle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DateOfBooking { get; set; }
    public DateTime? DateOfRelease { get; set; }
    public DateTime? DateOfReturn { get; set; }
    public long? StartMilage { get; set; }
    public long? EndMilage { get; set; }
    public double? Price { get; set; }
}

public readonly record struct BookingId(Guid Id)
{
    public static BookingId Empty => new(Guid.Empty);
    public static BookingId NewBookingId() => new(Guid.NewGuid());
}
