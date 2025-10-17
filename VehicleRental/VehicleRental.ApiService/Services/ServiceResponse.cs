using Microsoft.AspNetCore.Mvc;
using VehicleRental.Api.Models;

namespace VehicleRental.Api.Services;

public enum ServiceResponseType
{
    Invalid = 0,
    Success,
    NotFound,
    Created
}

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public ServiceResponseType ResponseType { get; set; }

    public ActionResult ToActionResult()
    {
        return ResponseType switch
        {
            ServiceResponseType.Success => new OkObjectResult(this),
            ServiceResponseType.NotFound => new NotFoundObjectResult(this),
            ServiceResponseType.Invalid => new BadRequestObjectResult(this),
            ServiceResponseType.Created => new CreatedAtActionResult(
                    actionName: nameof(ToActionResult),
                    controllerName: null,
                    routeValues: new { id = GetId() },
                    value: Data),
            _ => new OkObjectResult(this)
        };
    }

    private int GetId()
    {
        if (Data is DtoBase dtoBase)
            return dtoBase.Id;
        return 0;
    }

    public static ServiceResponse<T> SuccessResult(T data)
    {
        return new ServiceResponse<T>
        {
            Data = data,
            Success = true,
            ResponseType = ServiceResponseType.Success
        };
    }

    public static ServiceResponse<T> NotFoundResult(string message)
    {
        return new ServiceResponse<T>
        {
            Success = false,
            Message = message,
            ResponseType = ServiceResponseType.NotFound
        };
    }

    public static ServiceResponse<T> InvalidDataResult(string message)
    {
        return new ServiceResponse<T>
        {
            Success = false,
            Message = message,
            ResponseType = ServiceResponseType.Invalid
        };
    }

    public static ServiceResponse<T> SuccessFullyCreated(string message)
    {
        return new ServiceResponse<T>
        {
            Success = false,
            Message = message,
            ResponseType = ServiceResponseType.Created
        };
    }

}
