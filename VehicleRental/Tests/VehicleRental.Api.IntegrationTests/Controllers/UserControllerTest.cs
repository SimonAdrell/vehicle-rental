using System;
using System.Net.Http.Json;
using Microsoft.Identity.Client;
using VehicleRental.Api.Models;
using VehicleRental.Api.Tests;

namespace VehicleRental.Api.IntegrationTests.Controllers;

public class UserControllerTest : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserControllerTest(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task UserController_Requests_Valid()
    {
        // Arrange
        var createUserDto = new ClientCreateDto
        {
            IdentificationNumber = "id12321",
            Email = "testuser@example.com"
        };

        var updateUserDto = new ClientUpdateDto
        {
            IdentificationNumber = "id12321",
            Email = "testuseremail"
        };

        // Act

        var postResponse = await _client.PostAsJsonAsync("/api/v1/Client", createUserDto);
        var postResponseDto = await postResponse.Content.ReadFromJsonAsync<ClientDto>();

        var getResponse = await _client.GetFromJsonAsync<ClientDto>($"/api/v1/Client/{postResponseDto?.Id}");

        var putResponse = await _client.PutAsJsonAsync($"/api/v1/Client/{postResponseDto?.Id}", updateUserDto);
        var putResponseDto = await putResponse.Content.ReadFromJsonAsync<ClientDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/Client/{postResponseDto?.Id}");
        var deleteResponseDto = await deleteResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        postResponse.EnsureSuccessStatusCode();
        putResponse.EnsureSuccessStatusCode();
        deleteResponse.EnsureSuccessStatusCode();

        Assert.NotNull(postResponseDto);
        Assert.Equal(createUserDto.IdentificationNumber, postResponseDto.IdentificationNumber);
        Assert.Equal(createUserDto.Email, postResponseDto.Email);

        Assert.NotNull(getResponse);
        Assert.Equal(postResponseDto.Id, getResponse.Id);
        Assert.Equal(postResponseDto.IdentificationNumber, getResponse.IdentificationNumber);
        Assert.Equal(postResponseDto.Email, getResponse.Email);

        Assert.NotNull(putResponseDto);
        Assert.Equal(updateUserDto.IdentificationNumber, putResponseDto.IdentificationNumber);
        Assert.Equal(updateUserDto.Email, putResponseDto.Email);

        Assert.NotNull(deleteResponseDto);
        Assert.Equal(postResponseDto.Id, deleteResponseDto.Id);
        Assert.Equal(postResponseDto.IdentificationNumber, deleteResponseDto.IdentificationNumber);
        Assert.Equal(putResponseDto.Email, deleteResponseDto.Email);
    }
}
