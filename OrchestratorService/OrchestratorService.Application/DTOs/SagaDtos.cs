using OrchestratorService.Domain.Entities;

namespace OrchestratorService.Application.DTOs;

public record OrderSagaDto(
    Guid Id,
    Guid OrderId,
    Guid UserId,
    SagaStatus Status,
    string CurrentStep,
    decimal OrderAmount,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    string? ErrorMessage);
