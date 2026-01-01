using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Mappers;

public static class VehicleTypeMapper
{
    extension(VehicleTypeEntity entity)
    {
        public VehicleTypeDto ToDto() => new()
        {
            Id = entity.Id.Id,
            Name = entity.Name,
            Description = entity.Description,
            PricePerDay = entity.PricePerDay,
            DayMultiplier = entity.DayMultiplier,
            PricePerKilometer = entity.PricePerKilometer,
            KilometerMultiplier = entity.KilometerMultiplier
        };
    }

    extension(VehicleTypeCreateDto dto)
    {
        public VehicleTypeEntity ToEntity() => new VehicleTypeEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            PricePerDay = dto.PricePerDay,
            DayMultiplier = dto.DayMultiplier,
            PricePerKilometer = dto.PricePerKilometer,
            KilometerMultiplier = dto.KilometerMultiplier
        };
    }

    extension(VehicleTypeDto dto)
    {
        public VehicleTypeEntity ToEntity() => new VehicleTypeEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            PricePerDay = dto.PricePerDay,
            DayMultiplier = dto.DayMultiplier,
            PricePerKilometer = dto.PricePerKilometer,
            KilometerMultiplier = dto.KilometerMultiplier
        };
    }
}
