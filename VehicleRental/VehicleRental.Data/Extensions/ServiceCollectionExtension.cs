using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VehicleRental.Data.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddVehicleRentalDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add SQL Server DbContext
        services.AddDbContext<VehicleRentalDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("vehiclerentaldb")
                ?? throw new InvalidOperationException("Connection string 'vehiclerentaldb' not found.")));

        // Ensure database is created. In production, consider using migrations instead.
        using (ServiceProvider serviceProvider = services.BuildServiceProvider())
        {
            VehicleRentalDbContext dbContext = serviceProvider.GetRequiredService<VehicleRentalDbContext>();
            dbContext.Database.EnsureCreated();
        }

        return services;
    }

}
