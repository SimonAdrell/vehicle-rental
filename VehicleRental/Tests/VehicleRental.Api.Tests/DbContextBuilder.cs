using Microsoft.EntityFrameworkCore;
using VehicleRental.Data;

namespace VehicleRental.Api.Tests;

public static class DbContextBuilder
{
    public static VehicleRentalDbContext CreateInMemoryDbContext()
    {
        DbContextOptions<VehicleRentalDbContext> options = new DbContextOptionsBuilder<VehicleRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new VehicleRentalDbContext(options);

        return dbContext;
    }
}
