using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Tests.Services;

public class VehicleTypeServiceTests
{
    [Fact]
    public async Task GetAllVehicleTypes_ReturnsOkResult_WithListOfVehicleTypes()
    {
        // Arrange
        Data.VehicleRentalDbContext datacontext = DbContextBuilder.CreateInMemoryDbContext();

        var serviceUnderTest = new VehicleTypeService(datacontext);

        var request = new VehicleTypeCreateDto
        {
            Name = "Småbil",
            Description = "En liten och smidig bil",
            PricePerDay = 200,
        };

        // Act

        ServiceResponse<VehicleTypeDto> response = await serviceUnderTest.CreateVehicleTypeAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(ServiceResponseType.Created, response.ResponseType);
        Assert.NotNull(response.Data);
        VehicleTypeDto responseItem = Assert.IsType<VehicleTypeDto>(response.Data);

        Data.Enties.VehicleTypeEntity? createdVehicleType = datacontext.TypeOfVehicles.SingleOrDefault(v => v.Name == "Småbil");
        Assert.NotNull(createdVehicleType);
        Assert.Equal(responseItem.Id, createdVehicleType.Id);
        Assert.NotNull(createdVehicleType);
        Assert.Equal(request.Description, createdVehicleType.Description);
        Assert.Equal(request.PricePerDay, createdVehicleType.PricePerDay);
        Assert.Null(createdVehicleType.DayMultiplier);
        Assert.Null(createdVehicleType.PricePerKilometer);
    }

    [Fact]
    public async Task GetAllVehicleTypes_NoName_ReturnsInvalid()
    {
        // Arrange
        Data.VehicleRentalDbContext datacontext = DbContextBuilder.CreateInMemoryDbContext();

        var serviceUnderTest = new VehicleTypeService(datacontext);

        var request = new VehicleTypeCreateDto
        {
            Name = "",
            Description = "En liten och smidig bil",
            PricePerDay = 200,
        };

        // Act
        ServiceResponse<VehicleTypeDto> response = await serviceUnderTest.CreateVehicleTypeAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(ServiceResponseType.Invalid, response.ResponseType);
        Assert.NotNull(response.Message);
        Assert.Contains("Validation failed.", response.Message);
    }

    [Fact]
    public async Task GetAllVehicleTypes_NoPricePerDay_ReturnsInvalid()
    {
        // Arrange
        Data.VehicleRentalDbContext datacontext = DbContextBuilder.CreateInMemoryDbContext();

        var serviceUnderTest = new VehicleTypeService(datacontext);

        var request = new VehicleTypeCreateDto
        {
            Name = "Småbil",
            Description = "En liten och smidig bil",
            PricePerDay = -1,
        };

        // Act
        ServiceResponse<VehicleTypeDto> response = await serviceUnderTest.CreateVehicleTypeAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(ServiceResponseType.Invalid, response.ResponseType);
        Assert.NotNull(response.Message);
        Assert.Contains("Validation failed.", response.Message);
    }
}
