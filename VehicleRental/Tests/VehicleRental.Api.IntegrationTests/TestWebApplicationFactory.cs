using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using VehicleRental.Data;

namespace VehicleRental.Api.Tests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Configure test configuration first
        builder.ConfigureAppConfiguration((context, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:vehiclerentaldb"] = "Server=InMemory;Database=TestDb;Trusted_Connection=true;"
        }));

        builder.ConfigureServices((context, services) =>
        {
            // Remove ALL existing DbContext registrations
            var dbContextDescriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<VehicleRentalDbContext>) ||
                     d.ServiceType == typeof(VehicleRentalDbContext) ||
                     d.ImplementationType?.Name.Contains("VehicleRentalDbContext") == true)
                .ToList();

            foreach (ServiceDescriptor? descriptor in dbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<VehicleRentalDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                options.EnableSensitiveDataLogging();
            });

            // Remove Aspire service defaults for testing
            services.RemoveAll(typeof(Microsoft.Extensions.Hosting.IHostedService));
        });

        // Suppress logging during tests
        builder.ConfigureLogging(logging => logging.ClearProviders());
    }

    public async Task<VehicleRentalDbContext> GetDbContextAsync()
    {
        IServiceScope scope = Services.CreateScope();
        VehicleRentalDbContext context = scope.ServiceProvider.GetRequiredService<VehicleRentalDbContext>();
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    public async Task SeedTestDataAsync()
    {
        using VehicleRentalDbContext context = await GetDbContextAsync();

        // Clear existing data
        context.TypeOfVehicles.RemoveRange(context.TypeOfVehicles);
        await context.SaveChangesAsync();

        // Seed test data
        Data.Enties.VehicleTypeEntity[] vehicleTypes = new[]
        {
            new Data.Enties.VehicleTypeEntity
            {
                Id = 1,
                Name = "Sedan",
                Description = "Comfortable sedan for city driving"
            },
            new VehicleRental.Data.Enties.VehicleTypeEntity
            {
                Id = 2,
                Name = "SUV",
                Description = ""
            }
        };

        context.TypeOfVehicles.AddRange(vehicleTypes);
        await context.SaveChangesAsync();
    }
}
