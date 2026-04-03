using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add Qdrant vector database with persistence
var qdrant = builder.AddQdrant("qdrant")
    .WithDataVolume("qdrant-data")
    .WithHttpEndpoint(port: 6333, name: "http");

// Add PostgreSQL with pgvector extension for relational + vector operations
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("postgres-data")
    .WithHttpEndpoint(port: 5432, name: "tcp")
    .WithEnvironment("POSTGRES_DB", "convocontentbuddy")
    .WithEnvironment("POSTGRES_USER", "convocontentbuddy")
    .WithEnvironment("POSTGRES_PASSWORD", "convocontentbuddy");

var db = postgres.AddDatabase("convocontentbuddy");

// Add Redis for caching and SignalR backplane
var redis = builder.AddRedis("redis")
    .WithDataVolume("redis-data")
    .WithHttpEndpoint(port: 6379, name: "tcp");

// Add the API project with explicit endpoints, health checks, and container references
var api = builder.AddProject<Projects.ConvoContentBuddy_Api>("api")
    .WithHttpEndpoint(port: 5000, name: "http")
    .WithHttpsEndpoint(port: 5001, name: "https")
    .WithHealthCheck("/health")
    .WithReference(qdrant)
    .WithReference(db)
    .WithReference(redis)
    .WithEnvironment("QDRANT_SERVICE__HTTP", qdrant.GetEndpoint("http"))
    .WithEnvironment("POSTGRES_SERVICE__TCP", postgres.GetEndpoint("tcp"))
    .WithEnvironment("REDIS_SERVICE__TCP", redis.GetEndpoint("tcp"));

// Add the Frontend project with explicit endpoints, health checks, and API reference
var frontend = builder.AddProject<Projects.ConvoContentBuddy_Frontend>("frontend")
    .WithHttpEndpoint(port: 5002, name: "http")
    .WithHttpsEndpoint(port: 5003, name: "https")
    .WithHealthCheck("/health")
    .WithReference(api)
    .WithReference(redis)
    .WithEnvironment("API_SERVICE__HTTP", api.GetEndpoint("http"))
    .WithEnvironment("API_SERVICE__HTTPS", api.GetEndpoint("https"))
    .WithEnvironment("REDIS_SERVICE__TCP", redis.GetEndpoint("tcp"));

builder.Build().Run();
