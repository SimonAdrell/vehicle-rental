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

        // Utlämning av fordon
        var RegistrationNumber = "XYZ123";
        var KundPersonNr = "1234567890";
        var bilKategori = "Lastbil";
        var datmForUtlamning = DateTime.UtcNow;
        var matarstallningForUtlamning = 1000;

        // Återlämning av fordon
        var datumForAterlamning = DateTime.UtcNow.AddDays(30);
        var matarstallningForAterlamning = 1300;


        var createVehicleTypeDto = new VehicleTypeCreateDto
        {
            Name = bilKategori,
            PricePerDay = 200,
            DayMultiplier = 1.5,
            PricePerKilometer = 16,
            KilometerMultiplier = 1.5
        };

        var createVehicleDto = new VehicleCreateDto
        {
            RegistrationNumber = RegistrationNumber,
        };

        var createClientDto = new ClientCreateDto
        {
            IdentificationNumber = KundPersonNr
        };

        var createBookingDto = new BookingCreateDto
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(-2),
            StartMilage = 1000,
        };

        var bookingReleaseDto = new BookingReleaseDto
        {
            CurrentMilage = matarstallningForUtlamning,
            ReleaseDate = datmForUtlamning
        };

        var bookingReturnDto = new BookingReturnDto
        {
            Milage = matarstallningForAterlamning,
            DateOfReturn = datumForAterlamning
        };

        // Act  

        var vehicleTypeResponse = await _client.PostAsJsonAsync("/api/v1/VehicleType", createVehicleTypeDto);

        var vehicleTypeDto = await _client.GetFromJsonAsync<IEnumerable<VehicleTypeDto>>($"/api/v1/VehicleType?name={bilKategori}");
        var vehicleType = vehicleTypeDto!.FirstOrDefault();

        createVehicleDto.VehicleTypeId = vehicleType!.Id;

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
