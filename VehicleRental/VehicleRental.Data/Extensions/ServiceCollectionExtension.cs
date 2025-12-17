using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VehicleRental.Data.Extensions;

public static class ServiceExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddVehicleRentalDbServices(IConfiguration configuration, string environment)
        {
            services.AddDbContext<VehicleRentalDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("vehiclerentaldb")
                    ?? throw new InvalidOperationException("Connection string 'vehiclerentaldb' not found.")));

            // This should use migrations instread. But for simplicity, we use EnsureCreated here.
            if (environment != "Testing")
            {
                using var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<VehicleRentalDbContext>();
                dbContext.Database.EnsureCreated();
            }

            return services;
        }
    }
}
