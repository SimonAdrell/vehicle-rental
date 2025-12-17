using VehicleRental.Api.Models;
using VehicleRental.Api.Services;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Tests.Services;

public class BookingServiceTest
{
    [Fact]
    public async Task ReturnBookingAsync_ValidBookingId_ReturnsUpdatedBooking()
    {
        // Arrange
        var expectedPrice = 1380;

        Data.VehicleRentalDbContext datacontext = DbContextBuilder.CreateInMemoryDbContext();
        var vehicleType = new VehicleTypeEntity
        {
            Name = "Lastbil",
            Description = "För en smidig flytt",
            PricePerDay = 200,
            PricePerKilometer = 16,
            DayMultiplier = 1.5,
            KilometerMultiplier = 1.5
        };
        datacontext.TypeOfVehicles.Add(vehicleType);

        var vehicle = new VehicleEntity
        {
            TypeOfVehicleId = vehicleType.Id,
            RegistrationNumber = "ABC123",
        };

        var client = new ClientEntity
        {
            IdentificationNumber = "1234567890",
            Name = "Test Client",
            Email = ""
        };

        datacontext.TypeOfVehicles.Add(vehicleType);
        datacontext.Vehicles.Add(vehicle);
        datacontext.Clients.Add(client);

        await datacontext.SaveChangesAsync();

        var booking = new BookingEntity
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            DateOfBooking = DateTime.UtcNow.AddDays(-2),
            StartMilage = 1000,
            ClientId = client.Id,
            Client = client,
            VehicleId = vehicle.Id,
            Vehicle = vehicle,
            DateOfRelease = DateTime.UtcNow
        };
        datacontext.Bookings.Add(booking);
        await datacontext.SaveChangesAsync();

        var priceService = new PriceService();

        var bookingService = new BookingService(datacontext, priceService);

        var returnDto = new BookingReturnDto
        {
            Milage = 1020,
            DateOfReturn = DateTime.UtcNow.AddDays(3)
        };

        // Act

        var response = await bookingService.ReturnBookingAsync(booking.Id.Id, returnDto, CancellationToken.None);

        // Assert
        // (Verify that the returned BookingDto has updated mileage, return date, and calculated price)
        Assert.NotNull(response.Data);
        Assert.Equal(expectedPrice, response.Data.Price);
    }
}
