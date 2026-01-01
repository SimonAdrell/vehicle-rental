using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Mappers;

public static class ClientMapper
{
    extension(ClientEntity client)
    {
        public ClientDto ToDto() => new()
        {
            Id = client.Id.Value,
            IdentificationNumber = client.IdentificationNumber,
            Name = client.Name,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber
        };
    }

    extension(ClientCreateDto clientCreateDto)
    {
        public ClientEntity ToEntity() => new()
        {
            IdentificationNumber = clientCreateDto.IdentificationNumber,
            Name = clientCreateDto.Name,
            Email = clientCreateDto.Email,
            PhoneNumber = clientCreateDto.PhoneNumber
        };
    }
}
