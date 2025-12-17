using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Mappers;
using VehicleRental.Api.Models;
using VehicleRental.Data;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Services;

public interface IClientService
{
    Task<ServiceResponse<IEnumerable<ClientDto>>> GetAllClientsAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<ClientDto>> CreateClientAsync(ClientCreateDto clientCreateDto, CancellationToken cancellationToken);
    Task<ServiceResponse<ClientDto>> GetClientByIdAsync(Guid clientId, CancellationToken cancellationToken);
    Task<ServiceResponse<ClientDto>> UpdateClientAsync(Guid clientId, ClientUpdateDto clientUpdateDto, CancellationToken cancellationToken);
    Task<ServiceResponse<ClientDto>> DeleteClientAsync(Guid clientId, CancellationToken cancellationToken);
}

public class ClientService(VehicleRentalDbContext dbContext) : IClientService
{
    public async Task<ServiceResponse<IEnumerable<ClientDto>>> GetAllClientsAsync(CancellationToken cancellationToken)
    {
        var clients = await dbContext.Clients
            .ToListAsync(cancellationToken);

        return ServiceResponse<IEnumerable<ClientDto>>.Success(clients.Select(c => c.ToDto()));
    }

    public async Task<ServiceResponse<ClientDto>> CreateClientAsync(ClientCreateDto clientCreateDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(clientCreateDto.IdentificationNumber))
        {
            return ServiceResponse<ClientDto>.Invalid(
               "Could not create client.",
               new Dictionary<string, string[]>
               {
                   [Constants.ValidationErrors.IdentificationNumber] = ["Identification number is required."]
               }
           );
        }

        var client = await dbContext.Clients
            .Where(c => c.IdentificationNumber == clientCreateDto.IdentificationNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (client != null)
        {
            return ServiceResponse<ClientDto>.Conflict(
               "Could not create client.",
               new Dictionary<string, string[]>
               {
                   [Constants.ValidationErrors.IdentificationNumber] = ["A client with the same identification number already exists."]
               }
           );
        }

        var createdEntity = clientCreateDto.ToEntity();

        dbContext.Clients.Add(createdEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<ClientDto>.Created(createdEntity.ToDto());
    }

    public async Task<ServiceResponse<ClientDto>> GetClientByIdAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var id = new ClientId(clientId);
        if (id.Value == Guid.Empty)
        {
            return ServiceResponse<ClientDto>.Invalid("Could not get client.",
                new Dictionary<string, string[]>
                {
                    [Constants.ValidationErrors.Id] = ["Invalid Client Id."]
                }
            );
        }

        var client = await dbContext.Clients
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (client == null)
        {
            return ServiceResponse<ClientDto>.NotFound("Could not get client.");
        }

        return ServiceResponse<ClientDto>.Success(client.ToDto());
    }

    public async Task<ServiceResponse<ClientDto>> UpdateClientAsync(Guid clientId, ClientUpdateDto clientUpdateDto, CancellationToken cancellationToken)
    {
        var id = new ClientId(clientId);
        var existingClient = await dbContext.Clients
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingClient == null)
        {
            return ServiceResponse<ClientDto>.NotFound("Could not update client.");
        }

        var clientWithSameIdentificationNumber = await dbContext.Clients
            .Where(c => c.IdentificationNumber == clientUpdateDto.IdentificationNumber && c.Id != id)
            .FirstOrDefaultAsync(cancellationToken);

        if (clientWithSameIdentificationNumber != null)
        {
            return ServiceResponse<ClientDto>.Conflict(
               "Could not update client.",
               new Dictionary<string, string[]>
               {
                   [Constants.ValidationErrors.IdentificationNumber] = ["A client with the same identification number already exists."]
               }
           );
        }

        existingClient.IdentificationNumber = clientUpdateDto.IdentificationNumber;
        existingClient.Name = clientUpdateDto.Name;
        existingClient.Email = clientUpdateDto.Email;
        existingClient.PhoneNumber = clientUpdateDto.PhoneNumber;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<ClientDto>.Success(existingClient.ToDto());
    }

    public async Task<ServiceResponse<ClientDto>> DeleteClientAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var id = new ClientId(clientId);

        if (id.Value == Guid.Empty)
        {
            return ServiceResponse<ClientDto>.Invalid("Could not delete client.",
                new Dictionary<string, string[]>
                {
                    [Constants.ValidationErrors.Id] = ["Invalid Client Id."]
                }
            );
        }

        var client = await dbContext.Clients
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (client == null)
        {
            return ServiceResponse<ClientDto>.NotFound("Could not delete client.");
        }

        dbContext.Clients.Remove(client);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<ClientDto>.Success(client.ToDto());
    }
}
