using AlfredBotWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Register the typed HTTP client for the Backend API
builder.Services.AddHttpClient<AlfredApiClient>(client =>
{
    // The "backend" name matches the .WithReference(backend) in AppHost
    client.BaseAddress = new Uri("http://backend"); 
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
