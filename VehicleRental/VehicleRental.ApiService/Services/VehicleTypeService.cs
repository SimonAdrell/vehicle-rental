using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Mappers;
using VehicleRental.Api.Models;
using VehicleRental.Data;

namespace VehicleRental.Api.Services;

public interface IVehicleTypeService
{
    Task<ServiceResponse<IEnumerable<VehicleTypeDto>>> GetAllVehicleTypesAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> CreateVehicleTypeAsync(VehicleTypeCreateDto vehicleCreateDto, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> GetVehicleTypeByIdAsync(int id, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> UpdateVehicleTypeAsync(int id, VehicleTypeDto vehicleType, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> DeleteVehicleTypeAsync(int id, CancellationToken cancellationToken);
}

public class VehicleTypeService(VehicleRentalDbContext dbContext) : IVehicleTypeService
{

    public async Task<ServiceResponse<VehicleTypeDto>> CreateVehicleTypeAsync(VehicleTypeCreateDto vehicleCreateDto, CancellationToken cancellationToken)
    {
        var validationErrors = new Dictionary<string, string[]>();
        if (string.IsNullOrEmpty(vehicleCreateDto.Name))
        {
            validationErrors.Add("Name", ["Vehicle type name is required."]);
        }

        if (vehicleCreateDto.PricePerDay < 0)
        {
            validationErrors.Add("PricePerDay", ["Price per day must be at least 0."]);
        }

        if (validationErrors.Count != 0)
        {
            return ServiceResponse<VehicleTypeDto>.Invalid("Validation failed.", new Dictionary<string, object>
            {
                [Constants.ValidationErrors.ErrorExtensionsKey] = validationErrors
            });
        }

        Data.Enties.VehicleTypeEntity newVehicleType = vehicleCreateDto.ToEntity();

        dbContext.TypeOfVehicles.Add(newVehicleType);


        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleTypeDto>.Created(newVehicleType.ToDto());
    }

    public async Task<ServiceResponse<VehicleTypeDto>> DeleteVehicleTypeAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return ServiceResponse<VehicleTypeDto>.Invalid("Invalid vehicle type id.", new Dictionary<string, object>
            {
                [Constants.ValidationErrors.ErrorExtensionsKey] = new Dictionary<string, string[]>
                {
                    ["Id"] = ["Invalid vehicle type id."]
                }
            });
        }

        Data.Enties.VehicleTypeEntity? vehicleType = await dbContext.TypeOfVehicles
            .FirstOrDefaultAsync(vt => vt.Id == id, cancellationToken);

        if (vehicleType == null)
        {
            return ServiceResponse<VehicleTypeDto>.NotFound("Vehicle type not found.");
        }

        if (await dbContext.Vehicles.AnyAsync(v => v.TypeOfVehicleId == id, cancellationToken))
        {
            return ServiceResponse<VehicleTypeDto>.Invalid("Cannot delete vehicle type that is in use by vehicles.", new Dictionary<string, object>
            {
                [Constants.ValidationErrors.ErrorExtensionsKey] = new Dictionary<string, string[]>
                {
                    ["Id"] = ["Vehicle type is in use by existing vehicles."]
                }
            });
        }

        dbContext.TypeOfVehicles.Remove(vehicleType);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleTypeDto>.Success(vehicleType.ToDto());
    }

    public async Task<ServiceResponse<IEnumerable<VehicleTypeDto>>> GetAllVehicleTypesAsync(CancellationToken cancellationToken)
    {
        List<Data.Enties.VehicleTypeEntity> vehicleTypes = await dbContext.TypeOfVehicles
            .ToListAsync(cancellationToken);

        return ServiceResponse<IEnumerable<VehicleTypeDto>>.Success(
            vehicleTypes.Select(vt => vt.ToDto()));
    }

    public async Task<ServiceResponse<VehicleTypeDto>> GetVehicleTypeByIdAsync(int id, CancellationToken cancellationToken)
    {
        Data.Enties.VehicleTypeEntity? vehicleType = await dbContext.TypeOfVehicles
            .FirstOrDefaultAsync(vt => vt.Id == id, cancellationToken);

        if (vehicleType == null)
        {
            return ServiceResponse<VehicleTypeDto>.NotFound($"Could not find vehicle type with id {id}.");
        }

        return ServiceResponse<VehicleTypeDto>.Success(vehicleType.ToDto());
    }

    public async Task<ServiceResponse<VehicleTypeDto>> UpdateVehicleTypeAsync(int id, VehicleTypeDto vehicleType, CancellationToken cancellationToken)
    {
        var validationErrors = new Dictionary<string, string[]>();
        if (string.IsNullOrEmpty(vehicleType.Name))
        {
            validationErrors.Add("Name", ["Vehicle type name is required."]);
        }

        if (vehicleType.PricePerDay < 0)
        {
            validationErrors.Add("PricePerDay", ["Price per day must be at least 0."]);
        }

        if (validationErrors.Count != 0)
        {
            return ServiceResponse<VehicleTypeDto>.Invalid("Validation failed.", new Dictionary<string, object>
            {
                [Constants.ValidationErrors.ErrorExtensionsKey] = validationErrors
            });
        }

        Data.Enties.VehicleTypeEntity? existingVehicleType = await dbContext.TypeOfVehicles
            .FirstOrDefaultAsync(vt => vt.Id == id, cancellationToken);

        if (existingVehicleType == null)
        {
            return ServiceResponse<VehicleTypeDto>.NotFound($"Could not find vehicle type with id {id}.");
        }

        existingVehicleType.Name = vehicleType.Name;
        existingVehicleType.Description = vehicleType.Description;
        existingVehicleType.PricePerDay = vehicleType.PricePerDay;
        existingVehicleType.DayMultiplier = vehicleType.DayMultiplier;
        existingVehicleType.PricePerKilometer = vehicleType.PricePerKilometer;
        existingVehicleType.KilometerMultiplier = vehicleType.KilometerMultiplier;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleTypeDto>.Success(existingVehicleType.ToDto());
    }
}
