using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Tests.Services;

public class PriceServiceTests
{
    [Fact]
    public void CalculateRentalPriceAsync_SmallCarValidInput_CalculatesPrice()
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new VehicleTypeDto
        {
            Name = "Småbil",
            PricePerDay = 200,
            PricePerKilometer = 0,
        };
        int rentalDays = 3;
        double kilometersDriven = 150000;

        // sut
        double result = sut.CalculateRentalPrice(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(600, result);
    }

    [Fact]
    public void CalculateRentalPriceAsync_KombiValidInput_CalculatesPrice()
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new VehicleTypeDto
        {
            Name = "Kombi",
            PricePerDay = 200,
            DayMultiplier = 1.3,
            PricePerKilometer = 16,
        };
        int rentalDays = 3;
        double kilometersDriven = 20;

        // sut
        double result = sut.CalculateRentalPrice(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(1100, result);
    }

    [Fact]
    public void CalculateRentalPriceAsync_LastbilValidInput_CalculatesPrice()
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new VehicleTypeDto
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
        double result = sut.CalculateRentalPrice(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(1380, result);
    }


    [Theory]
    [InlineData(0, 20, 200)]
    [InlineData(3, 20, 600)]
    [InlineData(30, 20, 6000)]
    public void CalculateRentalPrice_TinyCar_CalculatesPrice(int rentalDays, int kilometersDriven, double expectedPrice)
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new VehicleTypeDto
        {
            Name = "Småbil",
            PricePerDay = 200,
        };

        // sut
        double result = sut.CalculateRentalPrice(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(expectedPrice, result);
    }


    [Theory]
    [InlineData(0, 20, 580)]
    [InlineData(3, 20, 1100)]
    [InlineData(30, 20, 8120)]
    public void CalculateRentalPrice_Kombi_CalculatesPrice(int rentalDays, int kilometersDriven, double expectedPrice)
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new VehicleTypeDto
        {
            Name = "Kombi",
            PricePerDay = 200,
            DayMultiplier = 1.3,
            PricePerKilometer = 16
        };

        // sut
        double result = sut.CalculateRentalPrice(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(expectedPrice, result);
    }

    [Theory]
    [InlineData(0, 20, 780)]
    [InlineData(3, 20, 1380)]
    [InlineData(30, 20, 9480)]
    public void CalculateRentalPrice_Lastbil_CalculatesPrice(int rentalDays, int kilometersDriven, double expectedPrice)
    {
        // Arrange
        var sut = new PriceService();
        var typeOfVehicle = new VehicleTypeDto
        {
            Name = "Lastbil",
            PricePerDay = 200,
            DayMultiplier = 1.5,
            PricePerKilometer = 16,
            KilometerMultiplier = 1.5
        };

        // sut
        double result = sut.CalculateRentalPrice(typeOfVehicle, rentalDays, kilometersDriven);

        // Assert
        Assert.Equal(expectedPrice, result);
    }

}
