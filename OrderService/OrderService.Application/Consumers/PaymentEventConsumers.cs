using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using OrderService.Domain.Events;

namespace OrderService.Application.Consumers;

public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentCompletedConsumer> _logger;

    public PaymentCompletedConsumer(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<PaymentCompletedConsumer> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Payment completed for Order {OrderId}, Amount: {Amount}", 
            message.OrderId, message.Amount);

        try
        {
            var order = await _orderRepository.GetByIdAsync(message.OrderId, context.CancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for payment completion", message.OrderId);
                return;
            }

            order.MarkAsPaid();
            await _orderRepository.UpdateAsync(order, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("Order {OrderId} marked as paid", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment completion for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}

public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentFailedConsumer> _logger;

    public PaymentFailedConsumer(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<PaymentFailedConsumer> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Payment failed for Order {OrderId}, Reason: {Reason}", 
            message.OrderId, message.Reason);

        try
        {
            var order = await _orderRepository.GetByIdAsync(message.OrderId, context.CancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for payment failure", message.OrderId);
                return;
            }

            order.Cancel();
            await _orderRepository.UpdateAsync(order, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("Order {OrderId} cancelled due to payment failure", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment failure for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}
