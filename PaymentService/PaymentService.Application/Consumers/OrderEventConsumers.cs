using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Events;

namespace PaymentService.Application.Consumers;

public class OrderConfirmedConsumer : IConsumer<OrderConfirmedEvent>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderConfirmedConsumer> _logger;

    public OrderConfirmedConsumer(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<OrderConfirmedConsumer> logger)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderConfirmedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Order confirmed: {OrderId}, Amount: {Amount}", 
            message.OrderId, message.TotalAmount);

        try
        {
            // Create payment for the confirmed order
            var payment = new Payment(
                message.OrderId,
                message.UserId,
                message.TotalAmount,
                PaymentMethod.CreditCard); // Default payment method

            await _paymentRepository.AddAsync(payment, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            // Publish payment created event
            await _publishEndpoint.Publish(new PaymentCreatedEvent(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                payment.CreatedAt),
                context.CancellationToken);

            _logger.LogInformation("Payment created for Order {OrderId}: {PaymentId}", 
                message.OrderId, payment.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}

public class OrderCancelledConsumer : IConsumer<OrderCancelledEvent>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<OrderCancelledConsumer> logger)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Order cancelled: {OrderId}", message.OrderId);

        try
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(message.OrderId, context.CancellationToken);

            if (payment == null)
            {
                _logger.LogWarning("No payment found for cancelled Order {OrderId}", message.OrderId);
                return;
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                payment.Refund();
                await _paymentRepository.UpdateAsync(payment, context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                // Publish refund event
                await _publishEndpoint.Publish(new PaymentRefundedEvent(
                    payment.Id,
                    payment.OrderId,
                    payment.Amount,
                    DateTime.UtcNow),
                    context.CancellationToken);

                _logger.LogInformation("Payment {PaymentId} refunded for cancelled Order {OrderId}", 
                    payment.Id, message.OrderId);
            }
            else
            {
                payment.MarkAsFailed("Order was cancelled");
                await _paymentRepository.UpdateAsync(payment, context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                _logger.LogInformation("Payment {PaymentId} failed info updated for cancelled Order {OrderId}", 
                    payment.Id, message.OrderId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order cancellation for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}
