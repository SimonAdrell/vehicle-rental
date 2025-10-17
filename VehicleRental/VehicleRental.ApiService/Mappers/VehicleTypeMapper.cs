using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Mappers;

public static class VehicleTypeMapper
{
    public static VehicleTypeDto ToDto(this VehicleTypeEntity entity)
    {
        return new VehicleTypeDto
        {
            Name = entity.Name,
            Description = entity.Description,
            PricePerDay = entity.PricePerDay,
            DayMultiplier = entity.DayMultiplier,
            PricePerKilometer = entity.PricePerKilometer,
            KilometerMultiplier = entity.KilometerMultiplier
        };
    }
    
    public static VehicleTypeEntity ToEntity(this VehicleTypeDto dto)
    {
        return new VehicleTypeEntity
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
