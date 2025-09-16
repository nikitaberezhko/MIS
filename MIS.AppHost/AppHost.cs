var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("mis-db");

var api = builder.AddProject<Projects.MIS_Api>("mis-api")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
