using MediatR;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Queries;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Handlers;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto?>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<PaymentDto?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment == null)
            return null;

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

public class GetPaymentByOrderIdQueryHandler : IRequestHandler<GetPaymentByOrderIdQuery, PaymentDto?>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByOrderIdQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<PaymentDto?> Handle(GetPaymentByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);

        if (payment == null)
            return null;

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

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, List<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetAllPaymentsQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<List<PaymentDto>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetAllAsync(cancellationToken);

        return payments.Select(MapToDto).ToList();
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

public class GetPaymentsByUserIdQueryHandler : IRequestHandler<GetPaymentsByUserIdQuery, List<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsByUserIdQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<List<PaymentDto>> Handle(GetPaymentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return payments.Select(MapToDto).ToList();
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

public class GetPaymentsByStatusQueryHandler : IRequestHandler<GetPaymentsByStatusQuery, List<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsByStatusQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<List<PaymentDto>> Handle(GetPaymentsByStatusQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByStatusAsync(request.Status, cancellationToken);

        return payments.Select(MapToDto).ToList();
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
