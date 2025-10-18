using Microsoft.AspNetCore.Mvc;
using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBookings()
    {
        ServiceResponse<IEnumerable<BookingDto>> response = await bookingService.GetAllBookingsAsync(HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

}
