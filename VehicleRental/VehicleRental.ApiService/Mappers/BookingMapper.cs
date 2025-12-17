using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Mappers;

public static class BookingMapper
{
    extension(BookingEntity booking)
    {
        public BookingDto ToDto() => new()
        {
            Id = booking.Id.Id,
            Vehicle = booking.Vehicle.ToApiModel(),
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            Client = booking.Client.ToDto(),
            DateOfBooking = booking.DateOfBooking,
            DateOfReturn = booking.DateOfReturn,
            DateOfRelease = booking.DateOfRelease,
            StartMilage = booking.StartMilage,
            EndMilage = booking.EndMilage,
            Price = booking.Price
        };
    }

    extension(BookingCreateDto bookingCreateDto)
    {
        public BookingEntity ToEntity(ClientEntity client, VehicleEntity vehicle) => new()
        {
            StartDate = bookingCreateDto.StartDate,
            EndDate = bookingCreateDto.EndDate,
            ClientId = client.Id,
            Client = client,
            Vehicle = vehicle,
            VehicleId = vehicle.Id,
            DateOfBooking = bookingCreateDto.DateOfBooking,
            DateOfReturn = bookingCreateDto.DateOfReturn,
            StartMilage = bookingCreateDto.StartMilage,
            EndMilage = bookingCreateDto.EndMilage
        };
    }
}
