using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Mappers;
using VehicleRental.Api.Models;
using VehicleRental.Data;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Services;

public interface IVehicleTypeService
{
    Task<ServiceResponse<IEnumerable<VehicleTypeDto>>> GetActiveVehicleTypesAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> CreateVehicleTypeAsync(VehicleTypeCreateDto vehicleCreateDto, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> GetVehicleTypeByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<VehicleTypeDto>>> GetVehicleTypeByNameAsync(string name, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> UpdateVehicleTypeAsync(Guid id, VehicleTypeDto vehicleType, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> DeleteVehicleTypeAsync(Guid id, CancellationToken cancellationToken);
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
            return ServiceResponse<VehicleTypeDto>.Invalid("Validation failed.", validationErrors);
        }

        VehicleTypeEntity vehicleTypeEntity = vehicleCreateDto.ToEntity();
        dbContext.TypeOfVehicles.Add(vehicleTypeEntity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleTypeDto>.Created(vehicleTypeEntity.ToDto());
    }

    public async Task<ServiceResponse<VehicleTypeDto>> DeleteVehicleTypeAsync(Guid id, CancellationToken cancellationToken)
    {
        var vehicleTypeId = new VehicleTypeId(id);

        var validationErrors = GetIdValidationErrors(vehicleTypeId);
        if (validationErrors.Count > 0)
        {
            return ServiceResponse<VehicleTypeDto>.Invalid("Invalid vehicle type id.", validationErrors);
        }

        VehicleTypeEntity? vehicleType = await dbContext.TypeOfVehicles
            .Where(d => !d.IsDeleted && d.Id == new VehicleTypeId(id))
            .FirstOrDefaultAsync(cancellationToken);

        if (vehicleType == null)
        {
            return ServiceResponse<VehicleTypeDto>.NotFound("Vehicle type not found.");
        }

        if (await dbContext.Vehicles.AnyAsync(v => v.TypeOfVehicleId == vehicleTypeId, cancellationToken))
        {
            return ServiceResponse<VehicleTypeDto>.Invalid("Cannot delete vehicle type that is in use by vehicles.", new Dictionary<string, string[]>
            {
                [Constants.ValidationErrors.Id] = ["Vehicle type is in use by existing vehicles."]
            });
        }

        vehicleType.IsDeleted = true;
        vehicleType.DateOfDeletion = DateTime.UtcNow;

        dbContext.TypeOfVehicles.Update(vehicleType);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleTypeDto>.Success(vehicleType.ToDto());
    }

    private static Dictionary<string, string[]> GetIdValidationErrors(VehicleTypeId vehicleTypeId) => vehicleTypeId switch
    {
        { Id: var id } when id == Guid.Empty => new Dictionary<string, string[]>
        {
            [Constants.ValidationErrors.Id] = ["Vehicle type id cannot be empty."]
        },
        { Id: _ } => []
    };

    public async Task<ServiceResponse<IEnumerable<VehicleTypeDto>>> GetActiveVehicleTypesAsync(CancellationToken cancellationToken)
    {
        List<VehicleTypeEntity> vehicleTypes = await dbContext.TypeOfVehicles
            .Where(d => !d.IsDeleted)
            .ToListAsync(cancellationToken);
        return ServiceResponse<IEnumerable<VehicleTypeDto>>.Success(
            vehicleTypes.Select(vt => vt.ToDto()));
    }

    public async Task<ServiceResponse<VehicleTypeDto>> GetVehicleTypeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var vehicleTypeId = new VehicleTypeId(id);
        VehicleTypeEntity? vehicleType = await dbContext.TypeOfVehicles
            .Where(d => !d.IsDeleted)
            .FirstOrDefaultAsync(vt => vt.Id == vehicleTypeId, cancellationToken);

        if (vehicleType == null)
        {
            return ServiceResponse<VehicleTypeDto>.NotFound($"Could not find vehicle type with id {id}.");
        }

        return ServiceResponse<VehicleTypeDto>.Success(vehicleType.ToDto());
    }

    public async Task<ServiceResponse<IEnumerable<VehicleTypeDto>>> GetVehicleTypeByNameAsync(string name, CancellationToken cancellationToken)
    {
        var vehicleTypes = await dbContext.TypeOfVehicles
            .Where(vt => vt.Name == name)
            .ToListAsync(cancellationToken);

        if (vehicleTypes == null || vehicleTypes.Count == 0)
        {
            return ServiceResponse<IEnumerable<VehicleTypeDto>>.NotFound($"Could not find vehicle type with name {name}.");
        }

        return ServiceResponse<IEnumerable<VehicleTypeDto>>.Success(vehicleTypes.Select(vt => vt.ToDto()));
    }

    public async Task<ServiceResponse<VehicleTypeDto>> UpdateVehicleTypeAsync(Guid id, VehicleTypeDto vehicleType, CancellationToken cancellationToken)
    {
        var vehicleTypeId = new VehicleTypeId(id);
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
            return ServiceResponse<VehicleTypeDto>.Invalid("Validation failed.", validationErrors);
        }

        VehicleTypeEntity? existingVehicleType = await dbContext.TypeOfVehicles
            .Where(d => !d.IsDeleted)
            .FirstOrDefaultAsync(vt => vt.Id == vehicleTypeId, cancellationToken);

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
