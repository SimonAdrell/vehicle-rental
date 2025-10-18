using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;
namespace VehicleRental.Api.Mappers;

public static class VehicleMapper
{
    public static VehicleDto ToApiModel(this VehicleEntity entity) => new()
    {
        Id = entity.Id,
        RegistrationNumber = entity.RegistrationNumber,
        Milage = entity.Milage,
        IsRemoved = entity.IsRemoved,
        VehicleTypeId = entity.TypeOfVehicleId,
        VehicleType = entity.TypeOfVehicle?.ToDto()
    };

    public static VehicleEntity ToEntity(this VehicleDto apiModel) => new()
    {
        RegistrationNumber = apiModel.RegistrationNumber,
        Milage = apiModel.Milage,
        IsRemoved = apiModel.IsRemoved,
        TypeOfVehicleId = apiModel.VehicleType?.Id,
        TypeOfVehicle = apiModel.VehicleType?.ToEntity()
    };

    public static VehicleEntity ToEntity(this VehicleCreateDto apiModel) => new()
    {
        RegistrationNumber = apiModel.RegistrationNumber,
        Milage = apiModel.Milage,
        TypeOfVehicleId = apiModel.VehicleTypeId
    };

}
