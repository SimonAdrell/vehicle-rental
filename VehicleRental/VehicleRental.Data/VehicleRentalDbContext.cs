using Microsoft.EntityFrameworkCore;
using VehicleRental.Data.Enties;

namespace VehicleRental.Data;

public class VehicleRentalDbContext : DbContext
{
    public VehicleRentalDbContext(DbContextOptions<VehicleRentalDbContext> options) : base(options)
    {
    }

    public DbSet<VehicleEntity> Vehicles { get; set; }
    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<BookingEntity> Bookings { get; set; }
    public DbSet<VehicleTypeEntity> TypeOfVehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Vehicle configuration
        modelBuilder.Entity<VehicleEntity>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.RegistrationNumber).IsRequired().HasMaxLength(20);
            entity.Property(v => v.Milage).IsRequired();
            entity.Property(v => v.IsRemoved).IsRequired().HasDefaultValue(false);

            // Unique constraint: Only one active vehicle per registration number
            entity.HasIndex(v => v.RegistrationNumber)
                  .IsUnique()
                  .HasFilter("[IsRemoved] = 0");

            // Relationship with TypeOfVehicle
            entity.HasOne(v => v.TypeOfVehicle)
                  .WithMany()
                  .HasForeignKey(v => v.TypeOfVehicleId)
                  .IsRequired();

            // Relationship with Bookings
            entity.HasMany<BookingEntity>(v => v.Bookings)
                  .WithOne(b => b.Vehicle)
                  .HasForeignKey(b => b.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Client configuration
        modelBuilder.Entity<ClientEntity>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.IdentificationNumber).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Name).HasMaxLength(100);
            entity.Property(c => c.Email).HasMaxLength(255);
            entity.Property(c => c.PhoneNumber).HasMaxLength(20);

            // Unique constraint for identification number
            entity.HasIndex(c => c.IdentificationNumber)
                  .IsUnique();

            // Relationship with Bookings
            entity.HasMany<BookingEntity>(c => c.Bookings)
                  .WithOne(b => b.Client)
                  .HasForeignKey(b => b.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Booking configuration
        modelBuilder.Entity<BookingEntity>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.StartDate).IsRequired();
            entity.Property(b => b.EndDate).IsRequired();
            entity.Property(b => b.DateOfBooking).IsRequired();
            entity.Property(b => b.DateOfReturn).IsRequired();
            entity.Property(b => b.Milage).IsRequired();

            // Relationships are configured in Vehicle and Client entities
        });

        // TypeOfVehicle configuration
        modelBuilder.Entity<VehicleTypeEntity>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Description).HasMaxLength(200);

            // Unique constraint for type name
            entity.HasIndex(t => t.Name)
                  .IsUnique();
        });
    }
}