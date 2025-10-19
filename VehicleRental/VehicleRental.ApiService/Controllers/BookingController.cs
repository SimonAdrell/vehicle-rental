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

    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
    {
        ServiceResponse<BookingDto> response = await bookingService.CreateBookingAsync(bookingCreateDto, HttpContext.RequestAborted);
        return response.ToCreatedResult<BookingController>(HttpContext);
    }

    [HttpPut("{bookingId}/release")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReleaseBooking(int bookingId, BookingReleaseDto bookingReleaseDto)
    {
        ServiceResponse<BookingDto> response = await bookingService.ReleaseBookingAsync(bookingId, bookingReleaseDto, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpPut("{bookingId}/return")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReturnBooking(int bookingId, BookingReturnDto bookingReturnDto)
    {
        ServiceResponse<BookingDto> response = await bookingService.ReturnBookingAsync(bookingId, bookingReturnDto, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }
}
