using MassTransit;
using Microsoft.Extensions.Logging;
using OrchestratorService.Infrastructure.Events;
using Shared.Contracts;

namespace OrchestratorService.Infrastructure.Consumers;

public class StockReservationFailedConsumer : IConsumer<IStockReservationFailedEvent>
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

    public async Task Consume(ConsumeContext<IStockReservationFailedEvent> context)
    {
        var message = context.Message;
        _logger.LogWarning("⚠️ [ORCHESTRATOR] StockReservationFailedEvent received for Order {OrderId}, Product {ProductId}, Reason: {Reason}",
            message.OrderId, message.ProductId, message.Reason);

        try
        {
            // Orchestrator Compensation Step 2: Stock reservation failed, need to refund payment using shared contract
            await _publishEndpoint.Publish<IRefundRequestedEvent>(new
            {
                OrderId = message.OrderId,
                Reason = $"Stock reservation failed: {message.Reason}",
                RequestedDate = DateTime.UtcNow
            },
            context.CancellationToken);

            _logger.LogInformation("📤 Published RefundRequestedEvent for Order {OrderId}", message.OrderId);

            // After requesting refund, cancel the order using shared contract
            await _publishEndpoint.Publish<IOrderCancelledEvent>(new
            {
                OrderId = message.OrderId,
                Reason = $"Stock unavailable: {message.Reason}",
                CancelledDate = DateTime.UtcNow
            },
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
