using PaymentService.Domain.Entities;

namespace PaymentService.Application.DTOs;

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    PaymentStatus Status,
    PaymentMethod PaymentMethod,
    string? TransactionId,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    string? FailureReason);

public record CreatePaymentDto(
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    PaymentMethod PaymentMethod);

public record ProcessPaymentDto(
    string TransactionId);
