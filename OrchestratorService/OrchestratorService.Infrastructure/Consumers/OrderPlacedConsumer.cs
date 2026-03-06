using MassTransit;
using Microsoft.Extensions.Logging;
using OrchestratorService.Infrastructure.Events;

namespace OrchestratorService.Infrastructure.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(
        IPublishEndpoint publishEndpoint,
        ILogger<OrderPlacedConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("🎯 [ORCHESTRATOR] OrderPlacedEvent received for Order {OrderId}", message.OrderId);

        try
        {
            // Orchestrator Step 1: Request payment processing
            await _publishEndpoint.Publish(new PaymentRequestedEvent(
                message.OrderId,
                message.UserId,
                message.TotalAmount,
                message.Items,
                DateTime.UtcNow),
                context.CancellationToken);

            _logger.LogInformation("📤 Published PaymentRequestedEvent for Order {OrderId}, Amount: {Amount}",
                message.OrderId, message.TotalAmount);
            _logger.LogInformation("✅ [SAGA STEP 1] Payment requested for Order {OrderId}", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing OrderPlacedEvent for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}
