IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Setup PosgresSql Container and database

var username = builder.AddParameter("postgres-username", secret: true);
var password = builder.AddParameter("postgres-password", secret: true);

var postgres = builder.AddPostgres("postgres", username, password)
        .WithDataVolume(isReadOnly: false);
var postgresdb = postgres.AddDatabase("vehiclerentaldb");

builder.AddProject<Projects.VehicleRental_Api>("apiservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithHttpHealthCheck("/health");

await builder.Build().RunAsync();
