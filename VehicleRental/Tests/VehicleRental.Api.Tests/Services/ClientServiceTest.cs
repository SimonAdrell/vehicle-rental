using VehicleRental.Api.Models;
using VehicleRental.Api.Services;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Tests.Services;

public class ClientServiceTest
{

    [Fact]
    public async Task GetAllClientsAsync_ReturnsAllClients_WhenClientsExist()
    {
        // Arrange
        var dbContext = await SetupTestDatabaseWithClients();
        var clientService = new ClientService(dbContext);

        // Act
        var result = await clientService.GetAllClientsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Success, result.ResponseType);
        Assert.NotNull(result.Data);

        var resultList = Assert.IsAssignableFrom<IEnumerable<ClientDto>>(result.Data);
        var resultListAsList = resultList.ToList();
        Assert.Equal(2, resultListAsList.Count);
        Assert.Contains(resultListAsList, c => c.IdentificationNumber == "1234567890");
        Assert.Contains(resultListAsList, c => c.IdentificationNumber == "12321");
    }

    [Fact]
    public async Task GetAllClientsAsync_ReturnsEmptyList_WhenNoClientsExist()
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);

        // Act
        var result = await clientService.GetAllClientsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Success, result.ResponseType);
        Assert.NotNull(result.Data);

        var resultList = Assert.IsAssignableFrom<IEnumerable<ClientDto>>(result.Data);
        Assert.Empty(resultList);
    }

    [Fact]
    public async Task CreateClientAsync_ReturnsCreated_WhenValidClient()
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);
        var createDto = new ClientCreateDto
        {
            IdentificationNumber = "9876543210",
            Name = "New Client",
            Email = "new.client@example.com",
            PhoneNumber = "+1234567890"
        };

        // Act
        var result = await clientService.CreateClientAsync(createDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Created, result.ResponseType);
        Assert.NotNull(result.Data);
        Assert.Equal("9876543210", result.Data.IdentificationNumber);
        Assert.Equal("New Client", result.Data.Name);
        Assert.Equal("new.client@example.com", result.Data.Email);
        Assert.Equal("+1234567890", result.Data.PhoneNumber);
        Assert.True(result.Data.Id > 0);
    }

    [Fact]
    public async Task CreateClientAsync_ReturnsInvalid_WhenIdentificationNumberIsNull()
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);
        var createDto = new ClientCreateDto
        {
            IdentificationNumber = null!,
            Name = "Test Client"
        };

        // Act
        var result = await clientService.CreateClientAsync(createDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Invalid, result.ResponseType);
        Assert.Equal("Could not create client.", result.Message);
        Assert.NotNull(result.Extensions);
    }

    [Fact]
    public async Task CreateClientAsync_ReturnsInvalid_WhenIdentificationNumberIsEmpty()
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);
        var createDto = new ClientCreateDto
        {
            IdentificationNumber = "",
            Name = "Test Client"
        };

        // Act
        var result = await clientService.CreateClientAsync(createDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Invalid, result.ResponseType);
        Assert.Equal("Could not create client.", result.Message);
    }

    [Fact]
    public async Task CreateClientAsync_ReturnsConflict_WhenIdentificationNumberAlreadyExists()
    {
        // Arrange
        var dbContext = await SetupTestDatabaseWithClients();
        var clientService = new ClientService(dbContext);
        var createDto = new ClientCreateDto
        {
            IdentificationNumber = "1234567890", // Already exists in test data
            Name = "Duplicate Client"
        };

        // Act
        var result = await clientService.CreateClientAsync(createDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Conflict, result.ResponseType);
        Assert.Equal("Could not create client.", result.Message);
        Assert.NotNull(result.Extensions);
    }

    [Fact]
    public async Task CreateClientAsync_CreatesClientWithMinimalData()
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);
        var createDto = new ClientCreateDto
        {
            IdentificationNumber = "MINIMAL123"
        };

        // Act
        var result = await clientService.CreateClientAsync(createDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Created, result.ResponseType);
        Assert.NotNull(result.Data);
        Assert.Equal("MINIMAL123", result.Data.IdentificationNumber);
        Assert.Null(result.Data.Name);
        Assert.Null(result.Data.Email);
        Assert.Null(result.Data.PhoneNumber);
    }


    [Fact]
    public async Task GetClientByIdAsync_ReturnsClient_WhenClientExists()
    {
        // Arrange
        var dbContext = await SetupTestDatabaseWithClients();
        var clientService = new ClientService(dbContext);

        // Act
        var result = await clientService.GetClientByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Success, result.ResponseType);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("1234567890", result.Data.IdentificationNumber);
    }

    [Fact]
    public async Task GetClientByIdAsync_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);

        // Act
        var result = await clientService.GetClientByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.NotFound, result.ResponseType);
        Assert.Equal("Could not get client.", result.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task GetClientByIdAsync_ReturnsInvalid_WhenClientIdIsInvalid(int invalidId)
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);

        // Act
        var result = await clientService.GetClientByIdAsync(invalidId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Invalid, result.ResponseType);
        Assert.Equal("Could not get client.", result.Message);
        Assert.NotNull(result.Extensions);
    }

    [Fact]
    public async Task UpdateClientAsync_ReturnsUpdatedClient_WhenValidUpdate()
    {
        // Arrange
        var dbContext = await SetupTestDatabaseWithClients();
        var clientService = new ClientService(dbContext);
        var updateDto = new ClientUpdateDto
        {
            IdentificationNumber = "UPDATED123",
            Name = "Updated Name",
            Email = "updated@example.com",
            PhoneNumber = "+9876543210"
        };

        // Act
        var result = await clientService.UpdateClientAsync(1, updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Success, result.ResponseType);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("UPDATED123", result.Data.IdentificationNumber);
        Assert.Equal("Updated Name", result.Data.Name);
        Assert.Equal("updated@example.com", result.Data.Email);
        Assert.Equal("+9876543210", result.Data.PhoneNumber);
    }

    [Fact]
    public async Task UpdateClientAsync_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);
        var updateDto = new ClientUpdateDto
        {
            IdentificationNumber = "NONEXISTENT",
            Name = "Test"
        };

        // Act
        var result = await clientService.UpdateClientAsync(999, updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.NotFound, result.ResponseType);
        Assert.Equal("Could not update client.", result.Message);
    }

    [Fact]
    public async Task UpdateClientAsync_ReturnsConflict_WhenIdentificationNumberAlreadyExists()
    {
        // Arrange
        var dbContext = await SetupTestDatabaseWithClients();
        var clientService = new ClientService(dbContext);
        var updateDto = new ClientUpdateDto
        {
            IdentificationNumber = "12321", // This exists on client ID 2
            Name = "Conflict Test"
        };

        // Act - Try to update client ID 1 with client ID 2's identification number
        var result = await clientService.UpdateClientAsync(1, updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Conflict, result.ResponseType);
        Assert.Equal("Could not update client.", result.Message);
        Assert.NotNull(result.Extensions);
    }

    [Fact]
    public async Task UpdateClientAsync_AllowsSameIdentificationNumber_WhenUpdatingSameClient()
    {
        // Arrange
        var dbContext = await SetupTestDatabaseWithClients();
        var clientService = new ClientService(dbContext);
        var updateDto = new ClientUpdateDto
        {
            IdentificationNumber = "1234567890", // Same as current
            Name = "Updated Name Only"
        };

        // Act
        var result = await clientService.UpdateClientAsync(1, updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Success, result.ResponseType);
        Assert.NotNull(result.Data);
        Assert.Equal("1234567890", result.Data.IdentificationNumber);
        Assert.Equal("Updated Name Only", result.Data.Name);
    }

    [Fact]
    public async Task DeleteClientAsync_ReturnsDeletedClient_WhenClientExists()
    {
        // Arrange
        var dbContext = await SetupTestDatabaseWithClients();
        var clientService = new ClientService(dbContext);

        // Act
        var result = await clientService.DeleteClientAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Success, result.ResponseType);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("1234567890", result.Data.IdentificationNumber);

        // Verify client is actually deleted from database
        var deletedClient = await dbContext.Clients.FindAsync(1);
        Assert.Null(deletedClient);
    }

    [Fact]
    public async Task DeleteClientAsync_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);

        // Act
        var result = await clientService.DeleteClientAsync(999, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.NotFound, result.ResponseType);
        Assert.Equal("Could not delete client.", result.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task DeleteClientAsync_ReturnsInvalid_WhenClientIdIsInvalid(int invalidId)
    {
        // Arrange
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();
        var clientService = new ClientService(dbContext);

        // Act
        var result = await clientService.DeleteClientAsync(invalidId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResponseType.Invalid, result.ResponseType);
        Assert.Equal("Could not delete client.", result.Message);
        Assert.NotNull(result.Extensions);
    }

    private static async Task<Data.VehicleRentalDbContext> SetupTestDatabaseWithClients()
    {
        var dbContext = DbContextBuilder.CreateInMemoryDbContext();

        var client1 = new ClientEntity
        {
            Id = 1,
            IdentificationNumber = "1234567890",
            Name = "Test Client 1",
            Email = "test.client1@example.com",
            PhoneNumber = "+1111111111"
        };

        var client2 = new ClientEntity
        {
            Id = 2,
            IdentificationNumber = "12321",
            Name = "Test Client 2",
            Email = "test.client2@example.com",
            PhoneNumber = "+2222222222"
        };

        dbContext.Clients.Add(client1);
        dbContext.Clients.Add(client2);
        await dbContext.SaveChangesAsync();
        return dbContext;
    }

}
