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

    public static BookingEntity ToEntity(this BookingCreateDto bookingCreateDto, ClientEntity client, VehicleEntity vehicle) => new()
    {
        StartDate = bookingCreateDto.StartDate,
        EndDate = bookingCreateDto.EndDate,
        ClientId = bookingCreateDto.ClientId,
        Client = client,
        Vehicle = vehicle,
        VehicleId = bookingCreateDto.VehicleId,
        DateOfBooking = bookingCreateDto.DateOfBooking,
        DateOfReturn = bookingCreateDto.DateOfReturn,
        Milage = bookingCreateDto.Milage!.Value
    };

}
