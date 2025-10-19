IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container

IResourceBuilder<ParameterResource> sqlPassword = builder.AddParameter("sql-password", secret: true);

IResourceBuilder<SqlServerDatabaseResource> sql = builder.AddSqlServer("sql", password: sqlPassword)
    .WithDataVolume("vehiclerental-sql-data")
    .AddDatabase("vehiclerentaldb");

builder.AddProject<Projects.VehicleRental_Api>("apiservice")
    .WithReference(sql)
    .WaitFor(sql)
    .WithHttpHealthCheck("/health");

await builder.Build().RunAsync();
