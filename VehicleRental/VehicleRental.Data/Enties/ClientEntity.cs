namespace VehicleRental.Data.Enties;

public record class ClientEntity
{
    public int Id { get; set; }
    public required string IdentificationNumber { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public IEnumerable<BookingEntity>? Bookings { get; set; }
}
