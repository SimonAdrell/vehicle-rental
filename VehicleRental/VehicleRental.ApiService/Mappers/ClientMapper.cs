using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Mappers;

public static class ClientMapper
{
    public static ClientDto ToDto(this ClientEntity client) =>
        new()
        {
            Id = client.Id,
            IdentificationNumber = client.IdentificationNumber,
            Name = client.Name,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber
        };

    public static ClientEntity ToEntity(this ClientCreateDto clientCreateDto) =>
        new()
        {
            IdentificationNumber = clientCreateDto.IdentificationNumber,
            Name = clientCreateDto.Name,
            Email = clientCreateDto.Email,
            PhoneNumber = clientCreateDto.PhoneNumber
        };

}
