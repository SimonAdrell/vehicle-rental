using Microsoft.EntityFrameworkCore;
using VehicleRental.Api.Mappers;
using VehicleRental.Api.Models;
using VehicleRental.Data;

namespace VehicleRental.Api.Services;

public interface IBookingService
{
    Task<ServiceResponse<IEnumerable<BookingDto>>> GetAllBookingsAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<BookingDto>> CreateBookingAsync(BookingCreateDto bookingCreateDto, CancellationToken cancellationToken);
    Task<ServiceResponse<BookingDto>> ReturnBookingAsync(int bookingId, BookingReturnDto bookingReturnDto, CancellationToken cancellationToken);
    Task<ServiceResponse<BookingDto>> ReleaseBookingAsync(int bookingId, BookingReleaseDto bookingReleaseDto, CancellationToken cancellationToken);
}

public class BookingService(VehicleRentalDbContext dbContext, IPriceService priceService) : IBookingService
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

        return ServiceResponse<BookingDto>.Created(createdBooking!.ToDto());
    }

    public async Task<ServiceResponse<BookingDto>> ReleaseBookingAsync(int bookingId, BookingReleaseDto bookingReleaseDto, CancellationToken cancellationToken)
    {
        var booking = await dbContext.Bookings
            .Include(b => b.Vehicle)
                .ThenInclude(v => v.TypeOfVehicle)
            .Include(b => b.Client)
            .Where(b => b.Id == bookingId)
            .FirstOrDefaultAsync(cancellationToken);

        if (booking == null)
        {
            return ServiceResponse<BookingDto>.NotFound("Booking not found.");
        }

        if (booking.DateOfRelease != null)
        {
            return ServiceResponse<BookingDto>.Failure("Booking is already released.");
        }

        booking.DateOfRelease = bookingReleaseDto.ReleaseDate;
        booking.StartMilage = bookingReleaseDto.CurrentMilage;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResponse<BookingDto>.Success(booking.ToDto());
    }

    public async Task<ServiceResponse<BookingDto>> ReturnBookingAsync(int bookingId, BookingReturnDto bookingReturnDto, CancellationToken cancellationToken)
    {
        var booking = await dbContext.Bookings
            .Include(b => b.Vehicle)
                .ThenInclude(v => v.TypeOfVehicle)
            .Include(b => b.Client)
            .Where(b => b.Id == bookingId)
            .FirstOrDefaultAsync(cancellationToken);

        if (booking == null)
        {
            return ServiceResponse<BookingDto>.NotFound("Booking not found.");
        }

        if (booking.DateOfRelease is null)
        {
            return ServiceResponse<BookingDto>.Failure("Booking is not released.");
        }

        booking.DateOfReturn = bookingReturnDto.DateOfReturn;
        booking.EndMilage = bookingReturnDto.Milage;

        var rentalDays = (booking.DateOfReturn!.Value - booking.DateOfRelease!.Value).Days;
        var totalMilage = booking.EndMilage.GetValueOrDefault() - booking.StartMilage.GetValueOrDefault();

        var vehicleTypeDto = booking.Vehicle.TypeOfVehicle?.ToDto();
        if (vehicleTypeDto == null)
        {
            return ServiceResponse<BookingDto>.Failure("Vehicle type not found.");
        }
        var price = priceService.CalculateRentalPrice(vehicleTypeDto, rentalDays, totalMilage);
        booking.Price = price;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResponse<BookingDto>.Success(booking.ToDto());
    }
}
