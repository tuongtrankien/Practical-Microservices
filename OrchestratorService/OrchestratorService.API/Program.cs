using OrchestratorService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add Orchestrator services (MassTransit + RabbitMQ + Consumers)
builder.Services.AddOrchestrator(builder.Configuration);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add health checks (optional)
builder.Services.AddHealthChecks();

var app = builder.Build();

// Simple health check endpoint
app.MapHealthChecks("/health");

app.MapGet("/", () => "Order Saga Orchestrator Service is running");

app.Run();
