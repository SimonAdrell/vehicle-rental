using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Mappers;
using VehicleRental.Api.Models;
using VehicleRental.Data;

namespace VehicleRental.Api.Services;

public interface IVehicleService
{
    Task<ServiceResponse<VehicleDto>> GetVehicleByIdAsync(int id);
    Task<ServiceResponse<VehicleDto>> GetVehicleRegistrationNumberAsync(string registrationNumber);
    Task<ServiceResponse<IEnumerable<VehicleDto>>> GetAllVehiclesAsync();
    Task<ServiceResponse<VehicleDto>> CreateVehicleAsync(VehicleDto vehicle);
    Task<ServiceResponse<VehicleDto>> UpdateVehicleAsync(VehicleDto vehicle);
    Task<ServiceResponse<VehicleDto>> DeleteVehicleAsync(int id);
}

public class VehicleService(VehicleRentalDbContext dbContext) : IVehicleService
{
    private const string NotFoundMessage = "Vehicle not found.";
    private readonly VehicleRentalDbContext _dbContext = dbContext;

    public async Task<ServiceResponse<VehicleDto>> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _dbContext.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return ServiceResponse<VehicleDto>.NotFoundResult(NotFoundMessage);
        }
        return new ServiceResponse<VehicleDto>
        {
            Data = vehicle.ToApiModel(),
            Success = true
        };
    }

    public async Task<ServiceResponse<IEnumerable<VehicleDto>>> GetAllVehiclesAsync()
    {
        var vehicles = await _dbContext.Vehicles.ToListAsync();
        return ServiceResponse<IEnumerable<VehicleDto>>.SuccessResult(vehicles.Select(v => v.ToApiModel()));
    }

    public async Task<ServiceResponse<VehicleDto>> CreateVehicleAsync(VehicleDto vehicle)
    {
        var entity = vehicle.ToEntity();
        _dbContext.Vehicles.Add(entity);
        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<VehicleDto>
        {
            Data = entity.ToApiModel(),
            Success = true
        };
    }
    public async Task<ServiceResponse<VehicleDto>> UpdateVehicleAsync(VehicleDto vehicle)
    {
        var existingVehicle = await _dbContext.Vehicles.FindAsync(vehicle.RegistrationNumber);
        if (existingVehicle is null)
            return ServiceResponse<VehicleDto>.NotFoundResult(NotFoundMessage);

        existingVehicle.Milage = vehicle.Milage;
        existingVehicle.IsRemoved = vehicle.IsRemoved;
        existingVehicle.TypeOfVehicleId = vehicle.TypeOfVehicleId;

        _dbContext.Vehicles.Update(existingVehicle);
        await _dbContext.SaveChangesAsync();
        return ServiceResponse<VehicleDto>.SuccessResult(existingVehicle.ToApiModel());
    }

    public async Task<ServiceResponse<VehicleDto>> DeleteVehicleAsync(int id)
    {
        var vehicle = await _dbContext.Vehicles.FindAsync(id);
        if (vehicle is null)
            return ServiceResponse<VehicleDto>.NotFoundResult("Vehicle not found.");

        _dbContext.Vehicles.Remove(vehicle);
        await _dbContext.SaveChangesAsync();
        return ServiceResponse<VehicleDto>.SuccessResult(vehicle.ToApiModel());
    }

    public async Task<ServiceResponse<VehicleDto>> GetVehicleRegistrationNumberAsync(string registrationNumber)
    {
        if(string.IsNullOrWhiteSpace(registrationNumber))
        {
            throw new ArgumentException("Registration number cannot be null or empty.", nameof(registrationNumber));
        }
        var vehicle = await _dbContext.Vehicles
                    .Include(v => v.TypeOfVehicle)
                    .Where(e => e.RegistrationNumber == registrationNumber)
                    .FirstOrDefaultAsync();

        if (vehicle is null)
        {
            return ServiceResponse<VehicleDto>.NotFoundResult("Vehicle not found.");
        }

        return ServiceResponse<VehicleDto>.SuccessResult(vehicle.ToApiModel());
    }
}
