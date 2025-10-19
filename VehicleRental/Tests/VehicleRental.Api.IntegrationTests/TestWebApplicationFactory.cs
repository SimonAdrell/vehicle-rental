using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VehicleRental.Data;

namespace VehicleRental.Api.Tests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Use a separate environment for testing
        builder.UseEnvironment("Testing");
        builder.ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:vehiclerentaldb"] = $"Server=InMemory;Database={_databaseName};Trusted_Connection=true;"
        }));

        return base.CreateHost(builder);
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            var dbContextDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<VehicleRentalDbContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor is not null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            services.AddDbContext<VehicleRentalDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
                options.EnableSensitiveDataLogging();
            });
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
