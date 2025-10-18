using System;
using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Mappers;
using VehicleRental.Api.Models;
using VehicleRental.Data;

namespace VehicleRental.Api.Services;

public interface IBookingService
{
    Task<ServiceResponse<IEnumerable<BookingDto>>> GetAllBookingsAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<BookingDto>> CreateBookingAsync(BookingCreateDto bookingCreateDto, CancellationToken cancellationToken);
}

public class BookingService(VehicleRentalDbContext dbContext) : IBookingService
{
    public async Task<ServiceResponse<IEnumerable<BookingDto>>> GetAllBookingsAsync(CancellationToken cancellationToken)
    {
        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Vehicle)
                .ThenInclude(v => v.TypeOfVehicle)
            .Include(b => b.Client)
            .ToListAsync(cancellationToken);

        return ServiceResponse<IEnumerable<BookingDto>>.Success(
            bookings.Select(b => b.ToDto())
        );
    }

    public async Task<ServiceResponse<BookingDto>> CreateBookingAsync(BookingCreateDto bookingCreateDto, CancellationToken cancellationToken)
    {


        var client = await dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == bookingCreateDto.ClientId, cancellationToken);

        if (client == null)
        {
            return ServiceResponse<BookingDto>.Failure("Client not found.");
        }

        var vehicle = await dbContext.Vehicles
            .FirstOrDefaultAsync(v => v.Id == bookingCreateDto.VehicleId, cancellationToken);

        if (vehicle == null)
        {
            return ServiceResponse<BookingDto>.Failure("Vehicle not found.");
        }

        var bookingEntity = bookingCreateDto.ToEntity(client, vehicle);

        dbContext.Bookings.Add(bookingEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        var createdBooking = await dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Vehicle)
                .ThenInclude(v => v.TypeOfVehicle)
            .Include(b => b.Client)
            .FirstOrDefaultAsync(b => b.Id == bookingEntity.Id, cancellationToken);

        return ServiceResponse<BookingDto>.Success(createdBooking!.ToDto());
    }

}
