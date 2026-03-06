namespace OrchestratorService.Domain.Events;

// Events consumed from other services
public record OrderPlacedEvent(Guid OrderId, Guid UserId, decimal TotalAmount, List<OrderItemInfo> Items, DateTime PlacedDate);

public record OrderItemInfo(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record StockReservedCompletedEvent(Guid OrderId, Guid ProductId, int ReservedQuantity, DateTime ReservedDate);

public record StockReservationFailedEvent(Guid OrderId, Guid ProductId, string Reason, DateTime FailedDate);

// Events published by orchestrator
public record StockReservationRequestedEvent(Guid OrderId, Guid ProductId, int Quantity, DateTime RequestedDate);

public record OrderConfirmedEvent(Guid OrderId, Guid UserId, decimal TotalAmount, DateTime ConfirmedDate);

public record OrderCancelledEvent(Guid OrderId, Guid UserId, DateTime CancelledDate, string Reason);
