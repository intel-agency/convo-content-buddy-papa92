var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components
builder.AddServiceDefaults();

var app = builder.Build();

app.MapGet("/", () => "Hello World! Welcome to ConvoContentBuddy");

app.MapDefaultEndpoints();

app.Run();
