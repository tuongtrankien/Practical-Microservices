using NotificationService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Notification Service is a pure event consumer - no controllers needed
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure layer services (RabbitMQ with event consumers)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Service = "NotificationService" })
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();
