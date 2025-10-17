# Vehicle Rental System - AI Agent Instructions

## Architecture Overview

This is a **Microsoft .NET Aspire** vehicle rental management system built with .NET 9. The solution follows a distributed architecture pattern with clear separation of concerns:

### Project Structure
- **`VehicleRental.AppHost`** - Aspire orchestration host that manages all services, service discovery, and health checks
- **`VehicleRental.ApiService`** - Web API backend for vehicle rental operations (referenced as `apiservice` in Aspire)
- **`VehicleRental.Web`** - Blazor Server frontend that consumes the API (referenced as `webfrontend` in Aspire)
- **`VehicleRental.Data`** - Entity models library with Entity Framework Core integration
- **`VehicleRental.ServiceDefaults`** - Shared Aspire service configuration (OpenTelemetry, health checks, resilience)

### Domain Model
Core entities in `VehicleRental.Data/Enties/` (note the typo in folder name):
- **`Booking`** - Central entity with redundant date fields (`StartDate`/`EndDate` AND `DateOfBooking`/`DateOfReturn`)
- **`Client`** - Customer with required `IdentificationNumber` and optional contact details
- **`Vehicle`** - Fleet vehicles with `RegistrationNumber`, `Milage` (note spelling), and `TypeOfVehicle` reference
- **`TypeOfVehicle`** - Vehicle categories (Car, Truck, etc.)

All entities use **record classes** with `int Id` primary keys and follow navigation property patterns.

## Development Workflows

### Running the Application
1. **Start via Aspire Host**: Run `VehicleRental.AppHost` project - this automatically orchestrates all services
2. **Aspire Dashboard**: Access at `https://localhost:17232` for service monitoring, logs, and telemetry
3. **API Service**: Automatically available at `https+http://apiservice` (service discovery)
4. **Web Frontend**: Automatically available with external endpoints, references API service

### Project Dependencies
- API Service depends on: `ServiceDefaults` only
- Web Frontend depends on: `ServiceDefaults` only  
- Data Layer: Standalone with `Microsoft.EntityFrameworkCore` v9.0.10
- **No direct cross-project references** - services communicate via HTTP with Aspire service discovery

### Key Patterns

#### Service Communication
```csharp
// Web frontend HttpClient configuration for API calls
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice"); // Aspire service discovery
});
```

#### Aspire Service Registration
```csharp
// AppHost.cs service orchestration
var apiService = builder.AddProject<Projects.VehicleRental_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.VehicleRental_Web>("webfrontend")
    .WithReference(apiService)  // Service dependency
    .WaitFor(apiService);       // Startup ordering
```

#### Service Defaults Pattern
All services call `builder.AddServiceDefaults()` which provides:
- OpenTelemetry tracing/metrics
- Health checks (`/health`, `/alive` endpoints)
- HTTP client resilience (Polly)
- Service discovery integration

## Critical Conventions

### Entity Modeling
- Use `record class` for all entities
- Required properties use `required` keyword: `public required string Name { get; set; }`
- Navigation collections are nullable: `IEnumerable<Booking>? Bookings`
- Note existing typos: `Milage` instead of `Mileage`, `Enties` folder name

### API Controllers
- Currently empty - inherit from `ControllerBase` with `[Route("api/[controller]")]`
- Located in `VehicleRental.ApiService/Controllers/`
- **Prefer controller-based APIs over minimal APIs** for clarity and organization
- Use standard REST patterns: GET, POST, PUT, DELETE methods

### Database & Data Access
- **SQL Server** is the target database (subject to change)
- Entity Framework Core 9.0.10 is already referenced in `VehicleRental.Data`
- No DbContext implemented yet - needs creation with proper connection string configuration
- Consider adding EF migrations support for database schema management

### Authentication & Security
- Authentication will be added in the future
- Plan for integration with Aspire service defaults
- Consider JWT or cookie-based authentication patterns

### Testing Strategy
- **Tests solution folder exists but is empty** - ready for test project creation
- Follow standard .NET testing patterns: Unit tests, Integration tests, API tests
- Consider using `Microsoft.AspNetCore.Mvc.Testing` for API integration tests
- Aspire supports test host scenarios for multi-service testing
- Test projects should reference appropriate service projects and use test databases

### Configuration & Health
- All services have health check endpoints (dev only)
- Aspire provides centralized configuration and observability
- Use `MapDefaultEndpoints()` for health check registration

## Development Notes

- **No database context implemented yet** - Data project only contains entity models
- **Controllers are scaffolded but empty** - business logic needs implementation
- **Aspire orchestration is fully configured** - focus on business logic, not infrastructure
- **Service discovery works via named references** - use `"apiservice"` not hardcoded URLs
- **OpenTelemetry is configured** - automatic observability for HTTP calls and metrics
- **Database flexibility** - SQL Server initially, but architecture should support future changes