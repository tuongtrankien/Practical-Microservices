using MassTransit;
using Microsoft.Extensions.Logging;
using OrchestratorService.Infrastructure.Events;
using Shared.Contracts;

namespace OrchestratorService.Infrastructure.Consumers;

public class PaymentFailedConsumer : IConsumer<IPaymentFailedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentFailedConsumer> _logger;

    public PaymentFailedConsumer(
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentFailedConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPaymentFailedEvent> context)
    {
        var message = context.Message;
        _logger.LogWarning("⚠️ [ORCHESTRATOR] PaymentFailedEvent received for Order {OrderId}, Reason: {Reason}",
            message.OrderId, message.Reason);

        try
        {
            // Orchestrator Compensation Step 1: Payment failed, cancel the order immediately using shared contract
            await _publishEndpoint.Publish<IOrderCancelledEvent>(new
            {
                OrderId = message.OrderId,
                Reason = $"Payment failed: {message.Reason}",
                CancelledDate = DateTime.UtcNow
            },
            context.CancellationToken);

            _logger.LogInformation("📤 Published OrderCancelledEvent for Order {OrderId}", message.OrderId);
            _logger.LogInformation("❌ [SAGA COMPENSATION] Order {OrderId} cancelled due to payment failure", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing PaymentFailedEvent for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}
