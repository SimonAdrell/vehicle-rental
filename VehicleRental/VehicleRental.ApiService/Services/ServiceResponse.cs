using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using VehicleRental.Api.Models;

namespace VehicleRental.Api.Services;

public enum ServiceResponseType
{
    Invalid = 0,
    Success,
    NotFound,
    Created,
    Failure,
    Conflict
}

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Extensions { get; set; }
    public ServiceResponseType ResponseType { get; set; }

    public ActionResult ToActionResult(HttpContext httpContext) => ResponseType switch
    {
        ServiceResponseType.Success => new OkObjectResult(Data),
        ServiceResponseType.NotFound => CreateProblemDetails(httpContext,
            StatusCodes.Status404NotFound,
            "Resource not found",
            Message),
        ServiceResponseType.Invalid => CreateProblemDetails(httpContext,
            StatusCodes.Status400BadRequest,
            "Invalid request",
            Message),
        ServiceResponseType.Failure => CreateProblemDetails(httpContext,
            StatusCodes.Status500InternalServerError,
            "Internal server error",
            Message),
        ServiceResponseType.Conflict => CreateProblemDetails(httpContext,
            StatusCodes.Status409Conflict,
            "Conflict",
            Message),
        _ => new OkObjectResult(Data)
    };

    private ObjectResult CreateProblemDetails(HttpContext httpContext, int statusCode, string title, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance =
                $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };


        if (Extensions != null)
        {
            problemDetails.Extensions[Constants.ValidationErrors.ErrorExtensionsKey] = Extensions;
        }

        problemDetails.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);

        Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        problemDetails.Extensions.TryAdd("traceId", activity?.Id);
        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    public ActionResult ToCreatedResult<TController>(HttpContext httpContext, string? actionName = null) where TController : ControllerBase
    {
        string controllerName = typeof(TController).Name.Replace("Controller", "");
        return ResponseType switch
        {
            ServiceResponseType.Created => new CreatedAtActionResult(
                                actionName: actionName,
                                controllerName: controllerName,
                                routeValues: new { id = GetId() },
                                value: Data),
            _ => ToActionResult(httpContext),
        };
    }

    private Guid GetId()
    {
        if (Data is DtoBase dtoBase)
        {
            return dtoBase.Id;
        }

        return Guid.Empty;
    }

    public static ServiceResponse<T> Success(T data) => new ServiceResponse<T>
    {
        Data = data,
        ResponseType = ServiceResponseType.Success
    };

    public static ServiceResponse<T> NotFound(string message) => new ServiceResponse<T>
    {
        Message = message,
        ResponseType = ServiceResponseType.NotFound
    };

    public static ServiceResponse<T> Failure(string message) => new ServiceResponse<T>
    {
        Message = message,
        ResponseType = ServiceResponseType.Failure
    };

    public static ServiceResponse<T> Invalid(string message, Dictionary<string, string[]> extensions) => new()
    {
        Message = message,
        ResponseType = ServiceResponseType.Invalid,
        Extensions = extensions
    };

    public static ServiceResponse<T> Conflict(string message, Dictionary<string, string[]> extensions) => new()
    {
        Message = message,
        ResponseType = ServiceResponseType.Conflict,
        Extensions = extensions
    };

    public static ServiceResponse<T> Created(T data) => new ServiceResponse<T>
    {
        Data = data,
        ResponseType = ServiceResponseType.Created
    };
    internal static ServiceResponse<VehicleDto> Invalid(string v, Dictionary<string, object> dictionary) => throw new NotImplementedException();
}
