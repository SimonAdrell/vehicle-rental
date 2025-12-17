using Microsoft.VisualBasic;
using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;
namespace VehicleRental.Api.Mappers;

public static class VehicleMapper
{
    extension(VehicleEntity entity)
    {
        public VehicleDto ToApiModel() => new()
        {
            Id = entity.Id.Id,
            RegistrationNumber = entity.RegistrationNumber,
            Milage = entity.Milage,
            IsRemoved = entity.IsRemoved,
            VehicleTypeId = entity.TypeOfVehicleId.Id,
            VehicleType = entity.TypeOfVehicle.ToDto()
        };
    }

    extension(VehicleDto apiModel)
    {
        public VehicleEntity ToEntity() => new()
        {
            RegistrationNumber = apiModel.RegistrationNumber,
            Milage = apiModel.Milage,
            IsRemoved = apiModel.IsRemoved,
            TypeOfVehicleId = new VehicleTypeId(apiModel.VehicleTypeId),
            TypeOfVehicle = apiModel.VehicleType.ToEntity()
        };
    }

    extension(VehicleCreateDto apiModel)
    {
        public VehicleEntity ToEntity(VehicleTypeEntity vehicleType) => new()
        {
            RegistrationNumber = apiModel.RegistrationNumber,
            Milage = apiModel.Milage,
            TypeOfVehicleId = new VehicleTypeId(apiModel.VehicleTypeId),
            TypeOfVehicle = vehicleType
        };
    }
}
