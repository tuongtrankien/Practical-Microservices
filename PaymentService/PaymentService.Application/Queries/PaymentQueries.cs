using MediatR;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Queries;

public record GetPaymentByIdQuery(Guid PaymentId) 
    : IRequest<PaymentDto?>;

public record GetPaymentByOrderIdQuery(Guid OrderId) 
    : IRequest<PaymentDto?>;

public record GetAllPaymentsQuery() 
    : IRequest<List<PaymentDto>>;

public record GetPaymentsByUserIdQuery(Guid UserId) 
    : IRequest<List<PaymentDto>>;

public record GetPaymentsByStatusQuery(PaymentStatus Status) 
    : IRequest<List<PaymentDto>>;
