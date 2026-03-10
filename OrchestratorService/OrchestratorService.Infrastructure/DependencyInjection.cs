using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchestratorService.Infrastructure.Consumers;

namespace OrchestratorService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrchestrator(this IServiceCollection services, IConfiguration configuration)
    {
        // MassTransit with RabbitMQ
        services.AddMassTransit(x =>
        {
            // Register consumers
            x.AddConsumer<OrderPlacedConsumer>();
            x.AddConsumer<PaymentSucceededConsumer>();
            x.AddConsumer<PaymentFailedConsumer>();
            x.AddConsumer<StockReservedCompletedConsumer>();
            x.AddConsumer<StockReservationFailedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfig = configuration.GetSection("RabbitMQ");
                cfg.Host(rabbitMqConfig["Host"], rabbitMqConfig["VirtualHost"], h =>
                {
                    h.Username(rabbitMqConfig["Username"]!);
                    h.Password(rabbitMqConfig["Password"]!);
                });

                // No need for SetEntityName - shared contracts handle message routing automatically

                // Configure endpoints for consumers
                cfg.ReceiveEndpoint("orchestrator-order-placed-queue", e =>
                {
                    e.ConfigureConsumer<OrderPlacedConsumer>(context);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("orchestrator-payment-succeeded-queue", e =>
                {
                    e.ConfigureConsumer<PaymentSucceededConsumer>(context);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("orchestrator-payment-failed-queue", e =>
                {
                    e.ConfigureConsumer<PaymentFailedConsumer>(context);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("orchestrator-stock-reserved-queue", e =>
                {
                    e.ConfigureConsumer<StockReservedCompletedConsumer>(context);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("orchestrator-stock-failed-queue", e =>
                {
                    e.ConfigureConsumer<StockReservationFailedConsumer>(context);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
