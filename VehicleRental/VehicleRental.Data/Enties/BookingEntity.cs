namespace VehicleRental.Data.Enties;

public record class BookingEntity
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ClientId { get; set; }
    public required ClientEntity Client { get; set; }
    public int VehicleId { get; set; }
    public required VehicleEntity Vehicle { get; set; }
    public DateTime DateOfBooking { get; set; }
    public DateTime DateOfReturn { get; set; }
    public long Milage { get; set; }
}
