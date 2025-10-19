using System;
using System.Net.Http.Json;
using VehicleRental.Api.Models;
using VehicleRental.Api.Tests;

namespace VehicleRental.Api.IntegrationTests.Controllers;

public class BookingControllerTest : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncDisposable
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;

    public BookingControllerTest(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task BookingController_Requests_Valid()
    {
        // Arrange

        var CreateVehicleTypeDto = new VehicleTypeCreateDto
        {
            Name = "Lastbil",
            PricePerDay = 200,
            DayMultiplier = 1.5,
            PricePerKilometer = 16,
            KilometerMultiplier = 1.5
        };

        var createVehicleDto = new VehicleCreateDto
        {
            VehicleTypeId = 1,
            RegistrationNumber = "ABC123",
        };

        var createClientDto = new ClientCreateDto
        {
            IdentificationNumber = "1234567890",
            Name = "Test Client",
        };

        var createBookingDto = new BookingCreateDto
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(-2),
            StartMilage = 1000,
        };

        var bookingReleaseDto = new BookingReleaseDto
        {
            CurrentMilage = 1000,
            ReleaseDate = DateTime.UtcNow
        };

        var bookingReturnDto = new BookingReturnDto
        {
            Milage = 1300,
            DateOfReturn = DateTime.UtcNow.AddDays(30)
        };

        // Act  

        var vehicleTypeResponse = await _client.PostAsJsonAsync("/api/v1/VehicleType", CreateVehicleTypeDto);
        var vehicleTypeResponseDto = await vehicleTypeResponse.Content.ReadFromJsonAsync<VehicleTypeDto>();

        createVehicleDto.VehicleTypeId = vehicleTypeResponseDto!.Id;

        var vehicleResponse = await _client.PostAsJsonAsync("/api/v1/Vehicle", createVehicleDto);
        var vehicleResponseDto = await vehicleResponse.Content.ReadFromJsonAsync<VehicleDto>();

        var clientResponse = await _client.PostAsJsonAsync("/api/v1/Client", createClientDto);
        var clientResponseDto = await clientResponse.Content.ReadFromJsonAsync<ClientDto>();

        createBookingDto.VehicleId = vehicleResponseDto!.Id;
        createBookingDto.ClientId = clientResponseDto!.Id;

        var bookingResponse = await _client.PostAsJsonAsync("/api/v1/Booking", createBookingDto);
        var bookingResponseDto = await bookingResponse.Content.ReadFromJsonAsync<BookingDto>();

        var releaseResponse = await _client.PutAsJsonAsync($"/api/v1/Booking/{bookingResponseDto!.Id}/Release", bookingReleaseDto);
        var releaseResponseDto = await releaseResponse.Content.ReadFromJsonAsync<BookingDto>();

        var returnResponse = await _client.PutAsJsonAsync($"/api/v1/Booking/{releaseResponseDto!.Id}/return", bookingReturnDto);
        var returnResponseDto = await returnResponse.Content.ReadFromJsonAsync<BookingDto>();

        // Assert

        vehicleTypeResponse.EnsureSuccessStatusCode();
        vehicleResponse.EnsureSuccessStatusCode();
        clientResponse.EnsureSuccessStatusCode();
        bookingResponse.EnsureSuccessStatusCode();
        releaseResponse.EnsureSuccessStatusCode();
        returnResponse.EnsureSuccessStatusCode();

        Assert.Equal(16200, returnResponseDto!.Price);
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
