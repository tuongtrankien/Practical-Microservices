using MassTransit;
using Microsoft.Extensions.Logging;
using OrchestratorService.Infrastructure.Events;

namespace OrchestratorService.Infrastructure.Consumers;

public class StockReservationFailedConsumer : IConsumer<StockReservationFailedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<StockReservationFailedConsumer> _logger;

    public StockReservationFailedConsumer(
        IPublishEndpoint publishEndpoint,
        ILogger<StockReservationFailedConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockReservationFailedEvent> context)
    {
        var message = context.Message;
        _logger.LogWarning("⚠️ [ORCHESTRATOR] StockReservationFailedEvent received for Order {OrderId}, Product {ProductId}, Reason: {Reason}",
            message.OrderId, message.ProductId, message.Reason);

        try
        {
            // Orchestrator Compensation Step 2: Stock reservation failed, need to refund payment
            // Note: In stateless saga, we don't have PaymentId stored
            // PaymentService should look up payment by OrderId for refund
            await _publishEndpoint.Publish(new RefundRequestedEvent(
                message.OrderId,
                Guid.Empty, // PaymentService will look up by OrderId
                0, // PaymentService will look up amount by OrderId
                DateTime.UtcNow),
                context.CancellationToken);

            _logger.LogInformation("📤 Published RefundRequestedEvent for Order {OrderId}", message.OrderId);

            // After requesting refund, cancel the order
            await _publishEndpoint.Publish(new OrderCancelledEvent(
                message.OrderId,
                DateTime.UtcNow,
                $"Stock unavailable: {message.Reason}"),
                context.CancellationToken);

            _logger.LogInformation("📤 Published OrderCancelledEvent for Order {OrderId}", message.OrderId);
            _logger.LogInformation("❌ [SAGA COMPENSATION] Order {OrderId} cancelled due to stock failure, refund requested", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing StockReservationFailedEvent for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}
