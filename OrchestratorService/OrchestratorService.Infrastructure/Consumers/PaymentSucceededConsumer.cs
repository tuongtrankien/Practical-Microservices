using MassTransit;
using Microsoft.Extensions.Logging;
using OrchestratorService.Infrastructure.Events;

namespace OrchestratorService.Infrastructure.Consumers;

public class PaymentSucceededConsumer : IConsumer<PaymentSucceededEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentSucceededConsumer> _logger;

    public PaymentSucceededConsumer(
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentSucceededConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("✅ [ORCHESTRATOR] PaymentSucceededEvent received for Order {OrderId}, PaymentId: {PaymentId}",
            message.OrderId, message.PaymentId);

        try
        {
            // Orchestrator Step 2: Payment succeeded, now request stock reservation for each item
            foreach (var item in message.Items)
            {
                await _publishEndpoint.Publish(new StockReservationRequestedEvent(
                    message.OrderId,
                    item.ProductId,
                    item.Quantity,
                    DateTime.UtcNow),
                    context.CancellationToken);

                _logger.LogInformation("📤 Published StockReservationRequestedEvent for Product {ProductId}, Quantity: {Quantity}",
                    item.ProductId, item.Quantity);
            }
            
            _logger.LogInformation("✅ [SAGA STEP 2] Payment succeeded, stock reservation requested for Order {OrderId}", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing PaymentSucceededEvent for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}
