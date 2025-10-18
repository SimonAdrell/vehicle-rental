using System;
using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Mappers;
using VehicleRental.Api.Models;
using VehicleRental.Data;

namespace VehicleRental.Api.Services;

public interface IBookingService
{
    Task<ServiceResponse<IEnumerable<BookingDto>>> GetAllBookingsAsync(CancellationToken cancellationToken);
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

}
