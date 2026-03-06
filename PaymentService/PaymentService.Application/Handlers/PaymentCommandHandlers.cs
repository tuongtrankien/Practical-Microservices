using MassTransit;
using MediatR;
using PaymentService.Application.Commands;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Events;

namespace PaymentService.Application.Handlers;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment(request.OrderId, request.UserId, request.Amount, request.PaymentMethod);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event to RabbitMQ
        await _publishEndpoint.Publish(new PaymentCreatedEvent(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.CreatedAt),
            cancellationToken);

        return MapToDto(payment);
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto(
            payment.Id,
            payment.OrderId,
            payment.UserId,
            payment.Amount,
            payment.Status,
            payment.PaymentMethod,
            payment.TransactionId,
            payment.CreatedAt,
            payment.ProcessedAt,
            payment.FailureReason);
    }
}

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new InvalidOperationException($"Payment with ID {request.PaymentId} not found");

        payment.MarkAsProcessing();
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish processing event
        await _publishEndpoint.Publish(new PaymentProcessingEvent(
            payment.Id,
            payment.OrderId,
            DateTime.UtcNow),
            cancellationToken);

        // Simulate payment processing and mark as completed
        payment.MarkAsCompleted(request.TransactionId);
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish completed event
        await _publishEndpoint.Publish(new PaymentCompletedEvent(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.ProcessedAt!.Value),
            cancellationToken);

        return MapToDto(payment);
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto(
            payment.Id,
            payment.OrderId,
            payment.UserId,
            payment.Amount,
            payment.Status,
            payment.PaymentMethod,
            payment.TransactionId,
            payment.CreatedAt,
            payment.ProcessedAt,
            payment.FailureReason);
    }
}

public class FailPaymentCommandHandler : IRequestHandler<FailPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public FailPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PaymentDto> Handle(FailPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new InvalidOperationException($"Payment with ID {request.PaymentId} not found");

        payment.MarkAsFailed(request.Reason);

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish failed event
        await _publishEndpoint.Publish(new PaymentFailedEvent(
            payment.Id,
            payment.OrderId,
            payment.FailureReason!,
            payment.ProcessedAt!.Value),
            cancellationToken);

        return MapToDto(payment);
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto(
            payment.Id,
            payment.OrderId,
            payment.UserId,
            payment.Amount,
            payment.Status,
            payment.PaymentMethod,
            payment.TransactionId,
            payment.CreatedAt,
            payment.ProcessedAt,
            payment.FailureReason);
    }
}

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public RefundPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PaymentDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new InvalidOperationException($"Payment with ID {request.PaymentId} not found");

        payment.Refund();

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish refund event
        await _publishEndpoint.Publish(new PaymentRefundedEvent(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            DateTime.UtcNow),
            cancellationToken);

        return MapToDto(payment);
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto(
            payment.Id,
            payment.OrderId,
            payment.UserId,
            payment.Amount,
            payment.Status,
            payment.PaymentMethod,
            payment.TransactionId,
            payment.CreatedAt,
            payment.ProcessedAt,
            payment.FailureReason);
    }
}
