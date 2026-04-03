var builder = DistributedApplication.CreateBuilder(args);

// Add the API project
var api = builder.AddProject<Projects.ConvoContentBuddy_Api>("api");

// Add the Frontend project
var frontend = builder.AddProject<Projects.ConvoContentBuddy_Frontend>("frontend")
    .WithReference(api);

builder.Build().Run();
