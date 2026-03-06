namespace OrderService.Domain.Events;

// Order events that will be published to RabbitMQ
public record OrderCreatedEvent(Guid OrderId, Guid UserId, decimal TotalAmount, DateTime OrderDate);

public record OrderConfirmedEvent(Guid OrderId, Guid UserId, decimal TotalAmount, DateTime ConfirmedDate);

public record OrderPaidEvent(Guid OrderId, Guid UserId, decimal TotalAmount, DateTime PaidDate);

public record OrderShippedEvent(Guid OrderId, Guid UserId, DateTime ShippedDate);

public record OrderDeliveredEvent(Guid OrderId, Guid UserId, DateTime DeliveredDate);

public record OrderCancelledEvent(Guid OrderId, Guid UserId, DateTime CancelledDate);

// External events that OrderService consumes
public record PaymentCompletedEvent(Guid PaymentId, Guid OrderId, decimal Amount, DateTime CompletedDate);

public record PaymentFailedEvent(Guid PaymentId, Guid OrderId, string Reason, DateTime FailedDate);
