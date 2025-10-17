using VehicleRental.Api.Models;
using VehicleRental.Data;
using VehicleRental.Api.Mappers;
using Microsoft.EntityFrameworkCore;

namespace VehicleRental.Api.Services;

public interface IVehicleTypeService
{
    Task<ServiceResponse<VehicleTypeDto>> GetVehicleTypeByIdAsync(int id, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> CreateVehicleTypeAsync(VehicleTypeDto vehicleType, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> UpdateVehicleTypeAsync(VehicleTypeDto vehicleType, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleTypeDto>> DeleteVehicleTypeAsync(int id, CancellationToken cancellationToken);
}

public class VehicleTypeService(VehicleRentalDbContext dbContext) : IVehicleTypeService
{
    public async Task<ServiceResponse<VehicleTypeDto>> CreateVehicleTypeAsync(VehicleTypeDto vehicleType, CancellationToken cancellationToken)
    {
        // Check name 
        if (string.IsNullOrEmpty(vehicleType.Name))
            return ServiceResponse<VehicleTypeDto>.InvalidDataResult("Vehicle type name is required.");

        if (vehicleType.PricePerDay < 0)
            return ServiceResponse<VehicleTypeDto>.InvalidDataResult("Price per day must be at least 0.");

        // Create vehicle type
        var newVehicleType = vehicleType.ToEntity();

        dbContext.TypeOfVehicles.Add(newVehicleType);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleTypeDto>.SuccessResult(newVehicleType.ToDto());
    }

    public Task<ServiceResponse<VehicleTypeDto>> DeleteVehicleTypeAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<VehicleTypeDto>>> GetAllVehicleTypesAsync(CancellationToken cancellationToken)
    {
        var vehicleTypes = await dbContext.TypeOfVehicles
            .ToListAsync(cancellationToken);

        return ServiceResponse<IEnumerable<VehicleTypeDto>>.SuccessResult(
            vehicleTypes.Select(vt => vt.ToDto()));
    }

    public async Task<ServiceResponse<VehicleTypeDto>> GetVehicleTypeByIdAsync(int id, CancellationToken cancellationToken  )
    {
        var vehicleType = await dbContext.TypeOfVehicles
            .FirstOrDefaultAsync(vt => vt.Id == id, cancellationToken);

        if (vehicleType == null)
            return ServiceResponse<VehicleTypeDto>.NotFoundResult("Vehicle type not found.");

        return ServiceResponse<VehicleTypeDto>.SuccessResult(vehicleType.ToDto());
    }

    public async Task<ServiceResponse<VehicleTypeDto>> UpdateVehicleTypeAsync(VehicleTypeDto vehicleType, CancellationToken cancellationToken)
    {
        // Check name 
        if (string.IsNullOrEmpty(vehicleType.Name))
            return ServiceResponse<VehicleTypeDto>.InvalidDataResult("Vehicle type name is required.");

        if (vehicleType.PricePerDay < 0)
            return ServiceResponse<VehicleTypeDto>.InvalidDataResult("Price per day must be at least 0.");

        // Update vehicle type
        var existingVehicleType = await dbContext.TypeOfVehicles
            .FirstOrDefaultAsync(vt => vt.Id == vehicleType.Id, cancellationToken);

        if (existingVehicleType == null)
            return ServiceResponse<VehicleTypeDto>.NotFoundResult("Vehicle type not found.");

        existingVehicleType.Name = vehicleType.Name;
        existingVehicleType.Description = vehicleType.Description;
        existingVehicleType.PricePerDay = vehicleType.PricePerDay;
        existingVehicleType.DayMultiplier = vehicleType.DayMultiplier;
        existingVehicleType.PricePerKilometer = vehicleType.PricePerKilometer;
        existingVehicleType.KilometerMultiplier = vehicleType.KilometerMultiplier;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleTypeDto>.SuccessResult(existingVehicleType.ToDto());
    }
}
