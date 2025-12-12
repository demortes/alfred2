var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume("postgres_data")
    .AddDatabase("alfreddb");

// Add MongoDB
var mongodb = builder.AddMongoDB("mongodb")
    .WithMongoExpress() // Web UI for MongoDB
    .WithDataVolume("mango_data")
    .AddDatabase("alfredaudit");

// Add Web API
var backend = builder.AddProject<Projects.AlfredBackend>("backend")
    .WithReference(postgres)
    .WithReference(mongodb)
    .WithExternalHttpEndpoints(); // Expose for frontend

// Add Angular Frontend
var frontend = builder.AddNpmApp("frontend", "../frontend")
    .WithReference(backend)
    .WithHttpEndpoint(port: 4200)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

// Add Bot Workers (scalable)
builder.AddProject<Projects.AlfredBotWorker>("bot-worker")
    .WithReference(backend)
    .WithReference(postgres)
    .WithReplicas(1); // Start 1 worker initially

builder.Build().Run();
