using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Infrastructure.Events;

namespace NotificationService.Infrastructure.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedConsumer> _logger;

    public ProductCreatedConsumer(ILogger<ProductCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("📦 New product created: {ProductName} (ID: {ProductId}), Price: ${Price}", 
            message.Name, message.ProductId, message.Price);

        // Simulate sending notification (email, SMS, push notification, etc.)
        await Task.Delay(100); // Simulate notification sending delay

        _logger.LogInformation("✅ Notification sent for product: {ProductName}", message.Name);
    }
}

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("🛒 New order created: Order {OrderId} for User {UserId}, Amount: ${TotalAmount}", 
            message.OrderId, message.UserId, message.TotalAmount);

        await Task.Delay(100);

        _logger.LogInformation("✅ Order confirmation email sent to user {UserId}", message.UserId);
    }
}

public class OrderConfirmedConsumer : IConsumer<OrderConfirmedEvent>
{
    private readonly ILogger<OrderConfirmedConsumer> _logger;

    public OrderConfirmedConsumer(ILogger<OrderConfirmedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderConfirmedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("✔️ Order confirmed: Order {OrderId} for User {UserId}", 
            message.OrderId, message.UserId);

        await Task.Delay(100);

        _logger.LogInformation("✅ Order confirmation notification sent to user {UserId}", message.UserId);
    }
}

public class OrderShippedConsumer : IConsumer<OrderShippedEvent>
{
    private readonly ILogger<OrderShippedConsumer> _logger;

    public OrderShippedConsumer(ILogger<OrderShippedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderShippedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("🚚 Order shipped: Order {OrderId} for User {UserId}", 
            message.OrderId, message.UserId);

        await Task.Delay(100);

        _logger.LogInformation("✅ Shipping notification sent to user {UserId}", message.UserId);
    }
}

public class OrderDeliveredConsumer : IConsumer<OrderDeliveredEvent>
{
    private readonly ILogger<OrderDeliveredConsumer> _logger;

    public OrderDeliveredConsumer(ILogger<OrderDeliveredConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderDeliveredEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("📬 Order delivered: Order {OrderId} for User {UserId}", 
            message.OrderId, message.UserId);

        await Task.Delay(100);

        _logger.LogInformation("✅ Delivery notification sent to user {UserId}", message.UserId);
    }
}

public class OrderCancelledConsumer : IConsumer<OrderCancelledEvent>
{
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(ILogger<OrderCancelledConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("❌ Order cancelled: Order {OrderId} for User {UserId}", 
            message.OrderId, message.UserId);

        await Task.Delay(100);

        _logger.LogInformation("✅ Cancellation notification sent to user {UserId}", message.UserId);
    }
}

public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly ILogger<PaymentCompletedConsumer> _logger;

    public PaymentCompletedConsumer(ILogger<PaymentCompletedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("💳 Payment completed: Payment {PaymentId} for Order {OrderId}, Amount: ${Amount}", 
            message.PaymentId, message.OrderId, message.Amount);

        await Task.Delay(100);

        _logger.LogInformation("✅ Payment confirmation notification sent for order {OrderId}", message.OrderId);
    }
}

public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly ILogger<PaymentFailedConsumer> _logger;

    public PaymentFailedConsumer(ILogger<PaymentFailedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("❌ Payment failed: Payment {PaymentId} for Order {OrderId}, Reason: {Reason}", 
            message.PaymentId, message.OrderId, message.Reason);

        await Task.Delay(100);

        _logger.LogInformation("✅ Payment failure notification sent for order {OrderId}", message.OrderId);
    }
}
