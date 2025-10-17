var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container

var sqlPassword = builder.AddParameter("sql-password", secret: true);

var sql = builder.AddSqlServer("sql", password: sqlPassword)
    .WithDataVolume("vehiclerental-sql-data") 
    .AddDatabase("vehiclerentaldb");

var apiService = builder.AddProject<Projects.VehicleRental_Api>("apiservice")
    .WithReference(sql)
    .WaitFor(sql)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.VehicleRental_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync();
