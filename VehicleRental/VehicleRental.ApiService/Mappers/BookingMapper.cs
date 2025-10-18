using System;
using VehicleRental.Api.Models;
using VehicleRental.Data.Enties;

namespace VehicleRental.Api.Mappers;

public static class BookingMapper
{
    public static BookingDto ToDto(this BookingEntity booking) => new()
        {
            Id = booking.Id,
            Vehicle = booking.Vehicle.ToApiModel(),
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            Client = booking.Client.ToDto(),
            DateOfBooking = booking.DateOfBooking,
            DateOfReturn = booking.DateOfReturn,
            Milage = booking.Milage
        };

}
