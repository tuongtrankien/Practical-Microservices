namespace NotificationService.Infrastructure.Events;

// Events from ProductService
public record ProductCreatedEvent(Guid ProductId, string Name, decimal Price, DateTime CreatedAt);

// Events from OrderService
public record OrderCreatedEvent(Guid OrderId, Guid UserId, decimal TotalAmount, DateTime OrderDate);
public record OrderConfirmedEvent(Guid OrderId, Guid UserId, decimal TotalAmount, DateTime ConfirmedDate);
public record OrderShippedEvent(Guid OrderId, Guid UserId, DateTime ShippedDate);
public record OrderDeliveredEvent(Guid OrderId, Guid UserId, DateTime DeliveredDate);
public record OrderCancelledEvent(Guid OrderId, Guid UserId, DateTime CancelledDate);

// Events from PaymentService
public record PaymentCompletedEvent(Guid PaymentId, Guid OrderId, decimal Amount, DateTime CompletedDate);
public record PaymentFailedEvent(Guid PaymentId, Guid OrderId, string Reason, DateTime FailedDate);
