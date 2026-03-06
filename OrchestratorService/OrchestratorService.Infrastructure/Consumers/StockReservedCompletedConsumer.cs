using MassTransit;
using Microsoft.Extensions.Logging;
using OrchestratorService.Infrastructure.Events;

namespace OrchestratorService.Infrastructure.Consumers;

public class StockReservedCompletedConsumer : IConsumer<StockReservedCompletedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<StockReservedCompletedConsumer> _logger;

    public StockReservedCompletedConsumer(
        IPublishEndpoint publishEndpoint,
        ILogger<StockReservedCompletedConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockReservedCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("✅ [ORCHESTRATOR] StockReservedCompletedEvent received for Order {OrderId}, Product {ProductId}",
            message.OrderId, message.ProductId);

        try
        {
            // Orchestrator Step 3: Stock reserved successfully, confirm the order
            await _publishEndpoint.Publish(new OrderConfirmedEvent(
                message.OrderId,
                DateTime.UtcNow),
                context.CancellationToken);

            _logger.LogInformation("📤 Published OrderConfirmedEvent for Order {OrderId}", message.OrderId);
            _logger.LogInformation("🎉 [SAGA SUCCESS] Order {OrderId} flow completed successfully", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing StockReservedCompletedEvent for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}
