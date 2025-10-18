using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VehicleRental.Data.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddVehicleRentalDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<VehicleRentalDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("vehiclerentaldb")
                ?? throw new InvalidOperationException("Connection string 'vehiclerentaldb' not found.")));

        return services;
    }

}
