var builder = DistributedApplication.CreateBuilder(args);

// Add the API project with explicit endpoints and health checks
var api = builder.AddProject<Projects.ConvoContentBuddy_Api>("api")
    .WithHttpEndpoint(port: 5000, name: "http")
    .WithHttpsEndpoint(port: 5001, name: "https")
    .WithHealthCheck("/health");

// Add the Frontend project with explicit endpoints, health checks, and API reference
var frontend = builder.AddProject<Projects.ConvoContentBuddy_Frontend>("frontend")
    .WithHttpEndpoint(port: 5002, name: "http")
    .WithHttpsEndpoint(port: 5003, name: "https")
    .WithHealthCheck("/health")
    .WithReference(api)
    .WithEnvironment("API_SERVICE__HTTP", api.GetEndpoint("http"))
    .WithEnvironment("API_SERVICE__HTTPS", api.GetEndpoint("https"));

builder.Build().Run();
