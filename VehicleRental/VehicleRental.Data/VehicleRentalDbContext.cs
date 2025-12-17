using Microsoft.EntityFrameworkCore;
using VehicleRental.Data.Enties;
using VehicleRental.Data.EntityTypeConfigurations;

namespace VehicleRental.Data;

public class VehicleRentalDbContext(DbContextOptions<VehicleRentalDbContext> options) : DbContext(options)
{
    public DbSet<VehicleEntity> Vehicles { get; set; }
    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<BookingEntity> Bookings { get; set; }
    public DbSet<VehicleTypeEntity> TypeOfVehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ClientEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BookingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleTypeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleEntityConfiguration());
    }
}
