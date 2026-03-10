using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Events;
using Shared.Contracts;

namespace PaymentService.Application.Consumers;

/// <summary>
/// Consumes PaymentRequestedEvent from OrchestratorService and processes payment
/// </summary>
public class PaymentRequestedConsumer : IConsumer<IPaymentRequestedEvent>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentRequestedConsumer> _logger;

    public PaymentRequestedConsumer(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentRequestedConsumer> logger)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPaymentRequestedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("💳 Payment requested for Order {OrderId}, Amount: {Amount}", 
            message.OrderId, message.Amount);

        try
        {
            // Create payment record
            var payment = new Payment(
                message.OrderId,
                message.UserId,
                message.Amount,
                PaymentMethod.CreditCard); // Default payment method

            await _paymentRepository.AddAsync(payment, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("Payment created: {PaymentId} for Order {OrderId}", payment.Id, message.OrderId);

            // Simulate payment processing
            payment.MarkAsProcessing();
            await _paymentRepository.UpdateAsync(payment, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            // Simulate payment processing delay
            await Task.Delay(500);

            // Simulate payment success (90% success rate)
            var random = new Random();
            var isSuccess = random.Next(1, 11) <= 9; // 90% success

            if (isSuccess)
            {
                // Payment succeeded
                payment.MarkAsCompleted($"TXN-{Guid.NewGuid().ToString()[..8]}");
                await _paymentRepository.UpdateAsync(payment, context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                // Publish IPaymentSucceededEvent to Orchestrator (using shared contract)
                await _publishEndpoint.Publish<IPaymentSucceededEvent>(new
                {
                    OrderId = message.OrderId,
                    PaymentId = payment.Id,
                    Amount = payment.Amount,
                    Items = message.Items.ToList<object>(), // Forward items from request for stock reservation
                    ProcessedDate = DateTime.UtcNow
                },
                context.CancellationToken);

                _logger.LogInformation("✅ Payment succeeded for Order {OrderId}, PaymentId: {PaymentId}", 
                    message.OrderId, payment.Id);
            }
            else
            {
                // Payment failed
                var reason = "Insufficient funds";
                payment.MarkAsFailed(reason);
                await _paymentRepository.UpdateAsync(payment, context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                // Publish IPaymentFailedEvent to Orchestrator (using shared contract)
                await _publishEndpoint.Publish<IPaymentFailedEvent>(new
                {
                    OrderId = message.OrderId,
                    Reason = reason,
                    FailedDate = DateTime.UtcNow
                },
                context.CancellationToken);

                // Publish legacy PaymentFailedEvent for NotificationService
                await _publishEndpoint.Publish(new PaymentFailedEvent(
                    payment.Id,
                    message.OrderId,
                    reason,
                    DateTime.UtcNow),
                    context.CancellationToken);

                _logger.LogWarning("❌ Payment failed for Order {OrderId}, Reason: {Reason}", 
                    message.OrderId, reason);
            }

            // Also publish legacy PaymentCompletedEvent/PaymentFailedEvent for NotificationService
            if (isSuccess)
            {
                await _publishEndpoint.Publish(new PaymentCompletedEvent(
                    payment.Id,
                    message.OrderId,
                    payment.Amount,
                    DateTime.UtcNow),
                    context.CancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment request for Order {OrderId}", message.OrderId);
            
            // Publish failure event using shared contract
            await _publishEndpoint.Publish<IPaymentFailedEvent>(new
            {
                OrderId = message.OrderId,
                Reason = $"Payment processing error: {ex.Message}",
                FailedDate = DateTime.UtcNow
            },
            context.CancellationToken);
            
            throw;
        }
    }
}

/// <summary>
/// Consumes RefundRequestedEvent from OrchestratorService and processes refund
/// </summary>
public class RefundRequestedConsumer : IConsumer<IRefundRequestedEvent>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RefundRequestedConsumer> _logger;

    public RefundRequestedConsumer(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<RefundRequestedConsumer> logger)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IRefundRequestedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("🔄 Refund requested for Order {OrderId}", message.OrderId);

        try
        {
            // Find payment by OrderId (since orchestrator may not have PaymentId)
            var payment = await _paymentRepository.GetByOrderIdAsync(message.OrderId, context.CancellationToken);

            if (payment == null)
            {
                _logger.LogWarning("No payment found for Order {OrderId} to refund", message.OrderId);
                return;
            }

            if (payment.Status != PaymentStatus.Completed)
            {
                _logger.LogWarning("Payment {PaymentId} for Order {OrderId} cannot be refunded. Status: {Status}",
                    payment.Id, message.OrderId, payment.Status);
                return;
            }

            // Process refund
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

            _logger.LogInformation("✅ Refund completed for Order {OrderId}, PaymentId: {PaymentId}, Amount: {Amount}",
                message.OrderId, payment.Id, payment.Amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for Order {OrderId}", message.OrderId);
            throw;
        }
    }
}
