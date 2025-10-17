using Microsoft.AspNetCore.Mvc;
using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Controllers
{
[Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class VehicleController(IVehicleService vehicleService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllVehicles()
        {
            var response = await vehicleService.GetAllVehiclesAsync();
            if(response.Success)
            {
                return Ok(response.Data);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicle = await vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return Ok(vehicle);
        }

        [HttpPost]
        [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleDto vehicleDto)
        {
            var createdVehicle = await vehicleService.CreateVehicleAsync(vehicleDto);
            return CreatedAtAction(nameof(GetVehicleById), new { id = createdVehicle }, createdVehicle);
        }
    }
}
