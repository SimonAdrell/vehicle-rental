using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Tests.Services;

public class PriceServiceTests
{
    [Fact]
    public async Task CalculateRentalPriceAsync_SmallCarValidInput_CalculatesPrice()
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new TypeOfVehicle
        {
            Name = "Småbil",
            PricePerDay = 200,
            PricePerKilometer = 0,
        };
        int rentalDays = 3;
        double kilometersDriven = 150000;

        // sut
        var result = await sut.CalculateRentalPriceAsync(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(600, result);
    }

    [Fact]
    public async Task CalculateRentalPriceAsync_KombiValidInput_CalculatesPrice()
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new TypeOfVehicle
        {
            Name = "Kombi",
            PricePerDay = 200,
            DayMultiplier = 1.3,
            PricePerKilometer = 16,
        };
        int rentalDays = 3;
        double kilometersDriven = 20;

        // sut
        var result = await sut.CalculateRentalPriceAsync(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(1100, result);
    }
    
           [Fact]
    public async Task CalculateRentalPriceAsync_LastbilValidInput_CalculatesPrice()
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new TypeOfVehicle
        {
            Name = "Lastbil",
            PricePerDay = 200,
            DayMultiplier = 1.5,
            PricePerKilometer = 16,
            KilometerMultiplier = 1.5
        };
        int rentalDays = 3;
        double kilometersDriven = 20;

        // sut
        var result = await sut.CalculateRentalPriceAsync(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(1380, result);
    }
}
