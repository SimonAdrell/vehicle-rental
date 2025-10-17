using Microsoft.AspNetCore.Mvc;
using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleTypeController(IVehicleTypeService vehicleTypeService) : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVehicleTypeById(int id)
        {
            var response = await vehicleTypeService.GetVehicleTypeByIdAsync(id, HttpContext.RequestAborted);
            return response.ToActionResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateVehicleType([FromBody] VehicleTypeDto vehicleTypeDto)
        {
            var response = await vehicleTypeService.CreateVehicleTypeAsync(vehicleTypeDto, HttpContext.RequestAborted);
            return response.ToActionResult();
        }
    }
}
