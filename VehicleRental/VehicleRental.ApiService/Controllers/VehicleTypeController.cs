using Microsoft.AspNetCore.Mvc;
using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class VehicleTypeController(IVehicleTypeService vehicleTypeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllVehicleTypes([FromQuery] string? name = null)
    {
          ServiceResponse<IEnumerable<VehicleTypeDto>> response = string.IsNullOrEmpty(name)
            ? await vehicleTypeService.GetActiveVehicleTypesAsync(HttpContext.RequestAborted)
            : await vehicleTypeService.GetVehicleTypeByNameAsync(name, HttpContext.RequestAborted);
        
        return response.ToActionResult(HttpContext);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateVehicleType([FromBody] VehicleTypeCreateDto vehicleTypeCreateDto)
    {
        ServiceResponse<VehicleTypeDto> response = await vehicleTypeService.CreateVehicleTypeAsync(vehicleTypeCreateDto, HttpContext.RequestAborted);
        return response.ToCreatedResult<VehicleTypeController>(HttpContext, nameof(GetVehicleTypeById));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVehicleTypeById(int id)
    {
        ServiceResponse<VehicleTypeDto> response = await vehicleTypeService.GetVehicleTypeByIdAsync(id, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateVehicleType(int id, [FromBody] VehicleTypeDto vehicleTypeDto)
    {
        ServiceResponse<VehicleTypeDto> response = await vehicleTypeService.UpdateVehicleTypeAsync(id, vehicleTypeDto, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteVehicleType(int id)
    {
        ServiceResponse<VehicleTypeDto> response = await vehicleTypeService.DeleteVehicleTypeAsync(id, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }
}
