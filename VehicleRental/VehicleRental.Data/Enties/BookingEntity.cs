namespace VehicleRental.Data.Enties;

public record class BookingEntity
{
    public int Id { get; set; }
    public required int ClientId { get; set; }
    public required ClientEntity Client { get; set; }
    public required int VehicleId { get; set; }
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
