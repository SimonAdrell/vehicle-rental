namespace VehicleRental.Data.Enties;

public record ClientEntity
{
    public ClientId Id { get; set; } = ClientId.NewClientId();
    public required string IdentificationNumber { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public IEnumerable<BookingEntity>? Bookings { get; set; }
}

public readonly record struct ClientId(Guid Value)
{
    public static ClientId Empty => new(Guid.Empty);

    public static ClientId NewClientId() => new(Guid.NewGuid());
}
