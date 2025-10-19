using Microsoft.AspNetCore.Mvc;
using VehicleRental.Api.Models;
using VehicleRental.Api.Services;

namespace VehicleRental.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ClientController(IClientService clientService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClientDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllClients()
    {
        ServiceResponse<IEnumerable<ClientDto>> response = await clientService.GetAllClientsAsync(HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpGet("{clientId}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientById(int clientId)
    {
        ServiceResponse<ClientDto> response = await clientService.GetClientByIdAsync(clientId, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateClient([FromBody] ClientCreateDto clientCreateDto)
    {
        ServiceResponse<ClientDto> response = await clientService.CreateClientAsync(clientCreateDto, HttpContext.RequestAborted);
        return response.ToCreatedResult<ClientController>(HttpContext);
    }

    [HttpPut("{clientId}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClient(int clientId, [FromBody] ClientUpdateDto clientUpdateDto)
    {
        ServiceResponse<ClientDto> response = await clientService.UpdateClientAsync(clientId, clientUpdateDto, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }

    [HttpDelete("{clientId}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClient(int clientId)
    {
        ServiceResponse<ClientDto> response = await clientService.DeleteClientAsync(clientId, HttpContext.RequestAborted);
        return response.ToActionResult(HttpContext);
    }
}
