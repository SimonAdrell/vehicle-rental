using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Mappers;

public static class VehicleMapper
{
    public static VehicleDto ToApiModel(this VehicleEntity entity) => new()
    {
        RegistrationNumber = entity.RegistrationNumber,
        Milage = entity.Milage,
        IsRemoved = entity.IsRemoved,
        TypeOfVehicleId = entity.TypeOfVehicleId
    };

    public static VehicleEntity ToEntity(this VehicleDto apiModel) => new()
    {
        RegistrationNumber = apiModel.RegistrationNumber,
        Milage = apiModel.Milage,
        IsRemoved = apiModel.IsRemoved,
        TypeOfVehicleId = apiModel.TypeOfVehicleId,
 TypeOfVehicle = null! // This will be set by EF Core based on the foreign key
    };

}
