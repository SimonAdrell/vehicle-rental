using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Models;
using VehicleRental.Api.Tests;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.IntegrationTests.Controllers;

public class VehicleTypeControllerTests : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncDisposable
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public VehicleTypeControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetAllVehicleTypes_ShouldReturnAllVehicleTypes_WhenDataExists()
    {
        // Arrange
        await _factory.SeedTestDataAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/v1/vehicletype");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        IEnumerable<VehicleTypeDto>? vehicleTypes = JsonSerializer.Deserialize<IEnumerable<VehicleTypeDto>>(content, _jsonOptions);

        Assert.NotNull(vehicleTypes);
        Assert.Equal(2, vehicleTypes.Count());
        Assert.Contains(vehicleTypes, vt => vt.Name == "Sedan");
        Assert.Contains(vehicleTypes, vt => vt.Name == "SUV");
    }

    [Fact]
    public async Task GetVehicleTypeById_ShouldReturnVehicleType_WhenExists()
    {
        // Arrange
        await _factory.SeedTestDataAsync();
        var dbContext = await _factory.GetDbContextAsync();
        var vehciletpyeDto = await dbContext.TypeOfVehicles.FirstOrDefaultAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync($"/api/v1/vehicletype/{vehciletpyeDto?.Id.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        VehicleTypeDto? vehicleType = JsonSerializer.Deserialize<VehicleTypeDto>(content, _jsonOptions);

        Assert.NotNull(vehicleType);
        Assert.Equal(vehciletpyeDto?.Id.Id, vehicleType!.Id);
        Assert.Equal("Sedan", vehicleType.Name);
        Assert.Equal("Comfortable sedan for city driving", vehicleType.Description);
    }

    [Fact]
    public async Task GetVehicleTypeById_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync($"/api/v1/vehicletype/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicleType_ShouldCreateAndReturnVehicleType_WhenValidData()
    {
        // Arrange
        var createDto = new VehicleTypeCreateDto
        {
            Name = "Truck",
            Description = "Heavy-duty truck for cargo transport",
            PricePerDay = 150.0
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/vehicletype", createDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        VehicleTypeDto? createdVehicleType = JsonSerializer.Deserialize<VehicleTypeDto>(content, _jsonOptions);

        Assert.NotNull(createdVehicleType);
        Assert.Equal("Truck", createdVehicleType!.Name);
        Assert.Equal("Heavy-duty truck for cargo transport", createdVehicleType.Description);

        // Verify in database
        using Data.VehicleRentalDbContext context = await _factory.GetDbContextAsync();
        Data.Enties.VehicleTypeEntity? dbVehicleType = await context.TypeOfVehicles
            .FirstOrDefaultAsync(vt => vt.Name == "Truck");
        Assert.NotNull(dbVehicleType);
        Assert.Equal("Heavy-duty truck for cargo transport", dbVehicleType!.Description);
    }

    [Fact]
    public async Task CreateVehicleType_ShouldReturnBadRequest_WhenInvalidData()
    {
        // Arrange
        var createDto = new VehicleTypeCreateDto
        {
            Name = "", // Invalid - empty name
            Description = "Test description",
            PricePerDay = 50.0
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/vehicletype", createDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVehicleType_ShouldUpdateAndReturnVehicleType_WhenValidData()
    {
        // Arrange
        await _factory.SeedTestDataAsync();
        var context = await _factory.GetDbContextAsync();

        var VehicleTypeEntity = new VehicleTypeEntity
        {
            Id = VehicleTypeId.NewVehicleTypeId(),
            Name = "Sedan",
            Description = "Comfortable sedan for city driving",
            PricePerDay = 70.0
        };

        context.TypeOfVehicles.Add(VehicleTypeEntity);
        await context.SaveChangesAsync();

        var updateDto = new VehicleTypeDto
        {
            Name = "Updated Sedan",
            Description = "Updated description for sedan",
            PricePerDay = 75.0
        };

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/v1/vehicletype/{VehicleTypeEntity.Id.Id}", updateDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        VehicleTypeDto? updatedVehicleType = JsonSerializer.Deserialize<VehicleTypeDto>(content, _jsonOptions);

        Assert.NotNull(updatedVehicleType);
        Assert.Equal("Updated Sedan", updatedVehicleType!.Name);
        Assert.Equal("Updated description for sedan", updatedVehicleType.Description);
    }

    [Fact]
    public async Task UpdateVehicleType_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        var updateDto = new VehicleTypeDto
        {
            Name = "Non-existent",
            Description = "This should not exist",
            PricePerDay = 100.0
        };

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/v1/vehicletype/{Guid.NewGuid()}", updateDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicleType_ShouldSetDeletedVehicleType_WhenExists()
    {
        // Arrange

        Data.VehicleRentalDbContext context = await _factory.GetDbContextAsync();
        await _factory.SeedTestDataAsync();
        var vehicleType = await context.TypeOfVehicles.FirstOrDefaultAsync();

        // Act

        HttpResponseMessage response = await _client.DeleteAsync($"/api/v1/vehicletype/{vehicleType?.Id.Id}");

        // Assert

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var deletedItem = await response.Content.ReadFromJsonAsync<VehicleTypeDto>();

        Assert.NotNull(deletedItem);
        Assert.Equal(vehicleType!.Id.Id, deletedItem!.Id);

        // Verify status updated

        Data.VehicleRentalDbContext secondContext = await _factory.GetDbContextAsync();

        VehicleTypeEntity? dbVehicleType = await secondContext.TypeOfVehicles.FirstOrDefaultAsync(t => t.Id == new VehicleTypeId(vehicleType!.Id.Id));

        Assert.NotNull(dbVehicleType);
        Assert.True(dbVehicleType.IsDeleted);
        Assert.NotNull(dbVehicleType.DateOfDeletion);
    }

    [Fact]
    public async Task DeleteVehicleType_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Act
        HttpResponseMessage response = await _client.DeleteAsync($"/api/v1/vehicletype/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicleType_ShouldReturnCreatedLocation_WhenSuccessful()
    {
        // Arrange
        var createDto = new VehicleTypeCreateDto
        {
            Name = "Motorcycle",
            Description = "Two-wheeled vehicle",
            PricePerDay = 45.0
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/vehicletype", createDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var createdObject = await response.Content.ReadFromJsonAsync<VehicleTypeDto>();
        Assert.NotNull(createdObject);
        Assert.Contains($"http://localhost/api/v1/VehicleType/{createdObject!.Id}", response.Headers.Location!.ToString());
    }

    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }
    }
}
