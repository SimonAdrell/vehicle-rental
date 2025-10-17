namespace VehicleRental.Data.Enties;

public record class VehicleEntity
{
    public int Id { get; set; }
    public required string RegistrationNumber { get; set; }
    public long Milage { get; set; } 
    public bool IsRemoved { get; set; }
    public int TypeOfVehicleId { get; set; }
    public required VehicleTypeEntity TypeOfVehicle { get; set; }
    public IEnumerable<BookingEntity>? Bookings { get; set; }
}
