using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Models;
using VehicleRental.Api.Tests;

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
        HttpResponseMessage response = await _client.GetAsync("/api/vehicletype");

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
    public async Task GetAllVehicleTypes_ShouldReturnEmptyList_WhenNoDataExists()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/vehicletype");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        IEnumerable<VehicleTypeDto>? vehicleTypes = JsonSerializer.Deserialize<IEnumerable<VehicleTypeDto>>(content, _jsonOptions);

        Assert.NotNull(vehicleTypes);
        Assert.Empty(vehicleTypes);
    }

    [Fact]
    public async Task GetVehicleTypeById_ShouldReturnVehicleType_WhenExists()
    {
        // Arrange
        await _factory.SeedTestDataAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/vehicletype/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        VehicleTypeDto? vehicleType = JsonSerializer.Deserialize<VehicleTypeDto>(content, _jsonOptions);

        Assert.NotNull(vehicleType);
        Assert.Equal(1, vehicleType!.Id);
        Assert.Equal("Sedan", vehicleType.Name);
        Assert.Equal("Comfortable sedan for city driving", vehicleType.Description);
    }

    [Fact]
    public async Task GetVehicleTypeById_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/vehicletype/999");

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
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/vehicletype", createDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        VehicleTypeDto? createdVehicleType = JsonSerializer.Deserialize<VehicleTypeDto>(content, _jsonOptions);

        Assert.NotNull(createdVehicleType);
        Assert.Equal("Truck", createdVehicleType!.Name);
        Assert.Equal("Heavy-duty truck for cargo transport", createdVehicleType.Description);
        Assert.True(createdVehicleType.Id > 0);

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
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/vehicletype", createDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVehicleType_ShouldUpdateAndReturnVehicleType_WhenValidData()
    {
        // Arrange
        await _factory.SeedTestDataAsync();

        var updateDto = new VehicleTypeDto
        {
            Id = 1,
            Name = "Updated Sedan",
            Description = "Updated description for sedan",
            PricePerDay = 75.0
        };

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync("/api/vehicletype/1", updateDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        VehicleTypeDto? updatedVehicleType = JsonSerializer.Deserialize<VehicleTypeDto>(content, _jsonOptions);

        Assert.NotNull(updatedVehicleType);
        Assert.Equal("Updated Sedan", updatedVehicleType!.Name);
        Assert.Equal("Updated description for sedan", updatedVehicleType.Description);

        // Verify in database
        using Data.VehicleRentalDbContext context = await _factory.GetDbContextAsync();
        Data.Enties.VehicleTypeEntity? dbVehicleType = await context.TypeOfVehicles.FindAsync(1);
        Assert.NotNull(dbVehicleType);
        Assert.Equal("Updated Sedan", dbVehicleType!.Name);
        Assert.Equal("Updated description for sedan", dbVehicleType.Description);
    }

    [Fact]
    public async Task UpdateVehicleType_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        var updateDto = new VehicleTypeDto
        {
            Id = 999,
            Name = "Non-existent",
            Description = "This should not exist",
            PricePerDay = 100.0
        };

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync("/api/vehicletype/999", updateDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicleType_ShouldDeleteVehicleType_WhenExists()
    {
        // Arrange
        await _factory.SeedTestDataAsync();

        // Act
        HttpResponseMessage response = await _client.DeleteAsync("/api/vehicletype/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify deletion in database
        using Data.VehicleRentalDbContext context = await _factory.GetDbContextAsync();
        Data.Enties.VehicleTypeEntity? dbVehicleType = await context.TypeOfVehicles.FindAsync(1);
        Assert.Null(dbVehicleType);
    }

    [Fact]
    public async Task DeleteVehicleType_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Act
        HttpResponseMessage response = await _client.DeleteAsync("/api/vehicletype/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/vehicletype")]
    [InlineData("/api/vehicletype/1")]
    public async Task VehicleTypeEndpoints_ShouldIncludeCorrectHeaders(string endpoint)
    {
        // Arrange
        await _factory.SeedTestDataAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(endpoint);

        // Assert
        Assert.True(response.Headers.Contains("Content-Type") ||
                   response.Content.Headers.ContentType != null);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task VehicleTypeEndpoints_ShouldReturnSuccessfully_WithBasicRouting()
    {
        // Arrange
        await _factory.SeedTestDataAsync();

        // Act - Test basic endpoint routing
        HttpResponseMessage response = await _client.GetAsync("/api/vehicletype");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/vehicletype", createDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("vehicletype", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task UpdateVehicleType_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        await _factory.SeedTestDataAsync();

        var updateDto = new VehicleTypeDto
        {
            Id = 2, // Different from URL parameter
            Name = "Mismatched ID",
            Description = "This should fail",
            PricePerDay = 80.0
        };

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync("/api/vehicletype/1", updateDto, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
