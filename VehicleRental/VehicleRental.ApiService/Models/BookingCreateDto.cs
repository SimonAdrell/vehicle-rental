using System;

namespace VehicleRental.Api.Models;

public record BookingCreateDto : DtoBase
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ClientId { get; set; }
    public int VehicleId { get; set; }
    public DateTime DateOfBooking { get; set; }
    public DateTime DateOfReturn { get; set; }
    public long? Milage { get; set; }
}
