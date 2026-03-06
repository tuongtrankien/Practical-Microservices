using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Infrastructure.Consumers;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure MassTransit with RabbitMQ and all event consumers
        services.AddMassTransit(x =>
        {
            // Register all consumers
            x.AddConsumer<ProductCreatedConsumer>();
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderConfirmedConsumer>();
            x.AddConsumer<OrderShippedConsumer>();
            x.AddConsumer<OrderDeliveredConsumer>();
            x.AddConsumer<OrderCancelledConsumer>();
            x.AddConsumer<PaymentCompletedConsumer>();
            x.AddConsumer<PaymentFailedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
