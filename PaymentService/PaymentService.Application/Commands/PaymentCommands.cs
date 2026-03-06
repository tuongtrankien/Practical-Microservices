using MediatR;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Commands;

public record CreatePaymentCommand(Guid OrderId, Guid UserId, decimal Amount, PaymentMethod PaymentMethod) 
    : IRequest<PaymentDto>;

public record ProcessPaymentCommand(Guid PaymentId, string TransactionId) 
    : IRequest<PaymentDto>;

public record FailPaymentCommand(Guid PaymentId, string Reason) 
    : IRequest<PaymentDto>;

public record RefundPaymentCommand(Guid PaymentId) 
    : IRequest<PaymentDto>;
