using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Mappers;
using VehicleRental.Api.Models;
using VehicleRental.Data;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Services;

public interface IVehicleService
{
    Task<ServiceResponse<VehicleDto>> GetVehicleByIdAsync(int vehicleId, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleDto>> GetVehicleRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<VehicleDto>>> GetAllVehiclesAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleDto>> CreateVehicleAsync(VehicleCreateDto vehicleCreateDto, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleDto>> UpdateVehicleAsync(int vehicleId, VehicleDto vehicle, CancellationToken cancellationToken);
    Task<ServiceResponse<VehicleDto>> DeleteVehicleAsync(int vehicleId, CancellationToken cancellationToken);
}

public class VehicleService(VehicleRentalDbContext dbContext) : IVehicleService
{
    private const string NotFoundMessage = "Vehicle not found.";
    private readonly VehicleRentalDbContext _dbContext = dbContext;

    public async Task<ServiceResponse<VehicleDto>> GetVehicleByIdAsync(int vehicleId, CancellationToken cancellationToken)
    {
        VehicleEntity? vehicle = await _dbContext.Vehicles
            .Include(v => v.TypeOfVehicle)
            .Where(e => e.Id == vehicleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (vehicle == null)
        {
            return ServiceResponse<VehicleDto>.NotFoundResult(NotFoundMessage);
        }

        return ServiceResponse<VehicleDto>.Success(vehicle.ToApiModel());
    }

    public async Task<ServiceResponse<IEnumerable<VehicleDto>>> GetAllVehiclesAsync(CancellationToken cancellationToken)
    {
        List<VehicleEntity> vehicles = await _dbContext.Vehicles
            .Include(v => v.TypeOfVehicle)
            .ToListAsync(cancellationToken);

        return ServiceResponse<IEnumerable<VehicleDto>>.Success(vehicles.Select(v => v.ToApiModel()));
    }

    public async Task<ServiceResponse<VehicleDto>> CreateVehicleAsync(VehicleCreateDto vehicleCreateDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(vehicleCreateDto.RegistrationNumber))
        {
            return ServiceResponse<VehicleDto>.Invalid(
               "Could not create vehicle.",
               new Dictionary<string, object>
               {
                   [Constants.ValidationErrors.ErrorExtensionsKey] = new Dictionary<string, string[]>
                   {
                       [Constants.ValidationErrors.RegistrationNumber] = ["Registration number is required."]
                   }
               }
           );
        }

        var existingVehicle = await _dbContext.Vehicles
            .Where(v => v.RegistrationNumber == vehicleCreateDto.RegistrationNumber && !v.IsRemoved)
                .FirstOrDefaultAsync(cancellationToken);
        if (existingVehicle != null)
        {
            return ServiceResponse<VehicleDto>.Conflict(
                        "Could not create vehicle.",
                        new Dictionary<string, object>
                        {
                            [Constants.ValidationErrors.ErrorExtensionsKey] = new Dictionary<string, string[]>
                            {
                                [Constants.ValidationErrors.RegistrationNumber] = ["Vehicle with the same registration number already exists."]
                            }
                        }
                    );
        }


        VehicleEntity entity = vehicleCreateDto.ToEntity();
        _dbContext.Vehicles.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var createdEntity = await _dbContext.Vehicles
            .Include(v => v.TypeOfVehicle)
            .Where(e => e.Id == entity.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (createdEntity is null)
        {
            return ServiceResponse<VehicleDto>.Failure("Created element could not be found.");
        }

        return ServiceResponse<VehicleDto>.Success(createdEntity.ToApiModel());
    }

    public async Task<ServiceResponse<VehicleDto>> UpdateVehicleAsync(int vehicleId, VehicleDto vehicle, CancellationToken cancellationToken)
    {

        if (string.IsNullOrEmpty(vehicle.RegistrationNumber))
        {
            return ServiceResponse<VehicleDto>.Invalid(
               "Could not update vehicle.",
               new Dictionary<string, object>
               {
                   [Constants.ValidationErrors.ErrorExtensionsKey] = new Dictionary<string, string[]>
                   {
                       [Constants.ValidationErrors.RegistrationNumber] = ["Registration number is required."]
                   }
               }
           );
        }

        VehicleEntity? existingVehicle = await _dbContext.Vehicles
            .Where(e => e.Id == vehicleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingVehicle is null)
        {
            return ServiceResponse<VehicleDto>.NotFoundResult(NotFoundMessage);
        }

        var existingVehicleWithRegNr = await _dbContext.Vehicles
            .Where(v => v.RegistrationNumber == vehicle.RegistrationNumber && !v.IsRemoved && existingVehicle.Id != vehicleId)
                .FirstOrDefaultAsync(cancellationToken);
        if (existingVehicleWithRegNr != null)
        {
            return ServiceResponse<VehicleDto>.Conflict(
                        "Could not update vehicle.",
                        new Dictionary<string, object>
                        {
                            [Constants.ValidationErrors.ErrorExtensionsKey] = new Dictionary<string, string[]>
                            {
                                [Constants.ValidationErrors.RegistrationNumber] = ["Vehicle with the same registration number already exists."]
                            }
                        }
                    );
        }


        var VehicleTypeEntity = await _dbContext.TypeOfVehicles
                .Where(vt => vt.Id == vehicle.VehicleTypeId)
                .FirstOrDefaultAsync(cancellationToken);

        if (VehicleTypeEntity is null)
        {
            return ServiceResponse<VehicleDto>.Invalid(
                "Could not update vehicle.",
                new Dictionary<string, object>
                {
                    [Constants.ValidationErrors.ErrorExtensionsKey] = new Dictionary<string, string[]>
                    {
                        ["VehicleType"] = ["Invalid Vehicle Type Id."]
                    }
                }
            );
        }

        existingVehicle.Milage = vehicle.Milage;
        existingVehicle.IsRemoved = vehicle.IsRemoved;
        existingVehicle.TypeOfVehicle = VehicleTypeEntity;

        _dbContext.Vehicles.Update(existingVehicle);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleDto>.Success(existingVehicle.ToApiModel());
    }

    public async Task<ServiceResponse<VehicleDto>> DeleteVehicleAsync(int vehicleId, CancellationToken cancellationToken)
    {
        VehicleEntity? vehicle = await _dbContext.Vehicles
            .Where(v => v.Id == vehicleId)
            .Include(v => v.TypeOfVehicle)
            .FirstOrDefaultAsync(cancellationToken);

        if (vehicle is null)
        {
            return ServiceResponse<VehicleDto>.NotFoundResult("Vehicle not found.");
        }

        _dbContext.Vehicles.Remove(vehicle);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResponse<VehicleDto>.Success(vehicle.ToApiModel());
    }

    public async Task<ServiceResponse<VehicleDto>> GetVehicleRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return ServiceResponse<VehicleDto>.Invalid("Could not get vehicle.",
                new Dictionary<string, object>
                {
                    [Constants.ValidationErrors.ErrorExtensionsKey] = new Dictionary<string, string[]>
                    {
                        ["RegistrationNumber"] = ["Registration number is required."]
                    }
                });
        }

        VehicleEntity? vehicle = await _dbContext.Vehicles
                    .Include(v => v.TypeOfVehicle)
                    .Where(e => e.RegistrationNumber == registrationNumber)
                    .FirstOrDefaultAsync(cancellationToken);

        if (vehicle is null)
        {
            return ServiceResponse<VehicleDto>.NotFoundResult("Vehicle not found.");
        }

        return ServiceResponse<VehicleDto>.Success(vehicle.ToApiModel());
    }
}
