IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container

IResourceBuilder<ParameterResource> sqlPassword = builder.AddParameter("sql-password", secret: true);

IResourceBuilder<SqlServerDatabaseResource> sql = builder.AddSqlServer("sql", password: sqlPassword)
    .WithDataVolume("vehiclerental-sql-data")
    .AddDatabase("vehiclerentaldb");

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.VehicleRental_Api>("apiservice")
    .WithReference(sql)
    .WaitFor(sql)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.VehicleRental_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync();
