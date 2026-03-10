using MassTransit;
using Microsoft.Extensions.Logging;
using OrchestratorService.Infrastructure.Events;
using Shared.Contracts;

namespace OrchestratorService.Infrastructure.Consumers;

public class OrderPlacedConsumer : IConsumer<IOrderPlacedEvent>
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

    public async Task Consume(ConsumeContext<IOrderPlacedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("🎯 [ORCHESTRATOR] OrderPlacedEvent received for Order {OrderId}", message.OrderId);

        try
        {
            // Orchestrator Step 1: Request payment processing using shared contract
            await _publishEndpoint.Publish<IPaymentRequestedEvent>(new
            {
                OrderId = message.OrderId,
                UserId = message.UserId,
                Amount = message.TotalAmount,
                Items = message.Items.ToList<object>(), // Forward items for stock reservation later
                RequestedDate = DateTime.UtcNow
            },
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
