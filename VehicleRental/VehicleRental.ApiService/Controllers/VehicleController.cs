using Microsoft.AspNetCore.Mvc;
using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Produces("application/json")]
public class VehicleController(IVehicleService vehicleService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllVehicles()
    {
        ServiceResponse<IEnumerable<VehicleDto>> response = await vehicleService.GetAllVehiclesAsync(HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpGet("{vehicleId}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVehicleById(int vehicleId)
    {
        ServiceResponse<VehicleDto> response = await vehicleService.GetVehicleByIdAsync(vehicleId, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateVehicle([FromBody] VehicleCreateDto vehicleCreateDto)
    {
        ServiceResponse<VehicleDto> response = await vehicleService.CreateVehicleAsync(vehicleCreateDto, HttpContext.RequestAborted);
        return response.ToCreatedResult<VehicleController>(HttpContext);
    }

    [HttpPut("{vehicleId}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] VehicleDto vehicleDto)
    {
        ServiceResponse<VehicleDto> response = await vehicleService.UpdateVehicleAsync(vehicleId, vehicleDto, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpDelete("{vehicleId}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVehicle(int vehicleId)
    {
        ServiceResponse<VehicleDto> response = await vehicleService.DeleteVehicleAsync(vehicleId, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

}
