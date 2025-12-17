using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;
namespace VehicleRental.Api.Mappers;

public static class VehicleMapper
{
    public static VehicleDto ToApiModel(this VehicleEntity entity) => new()
    {
        Id = entity.Id.Id,
        RegistrationNumber = entity.RegistrationNumber,
        Milage = entity.Milage,
        IsRemoved = entity.IsRemoved,
        VehicleTypeId = entity.TypeOfVehicleId.Id,
        VehicleType = entity.TypeOfVehicle?.ToDto()
    };

    public static VehicleEntity ToEntity(this VehicleDto apiModel) => new()
    {
        RegistrationNumber = apiModel.RegistrationNumber,
        Milage = apiModel.Milage,
        IsRemoved = apiModel.IsRemoved,
        TypeOfVehicleId = new VehicleTypeId(apiModel.VehicleTypeId),
        TypeOfVehicle = apiModel.VehicleType?.ToEntity()
    };

    public static VehicleEntity ToEntity(this VehicleCreateDto apiModel) => new()
    {
        RegistrationNumber = apiModel.RegistrationNumber,
        Milage = apiModel.Milage,
        TypeOfVehicleId = new VehicleTypeId(apiModel.VehicleTypeId)
    };

}
