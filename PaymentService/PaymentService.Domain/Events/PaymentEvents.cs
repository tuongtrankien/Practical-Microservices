namespace PaymentService.Domain.Events;

// Payment events that will be published to RabbitMQ
public record PaymentCreatedEvent(Guid PaymentId, Guid OrderId, decimal Amount, DateTime CreatedDate);

public record PaymentProcessingEvent(Guid PaymentId, Guid OrderId, DateTime ProcessingDate);

public record PaymentCompletedEvent(Guid PaymentId, Guid OrderId, decimal Amount, DateTime CompletedDate);

public record PaymentFailedEvent(Guid PaymentId, Guid OrderId, string Reason, DateTime FailedDate);

public record PaymentRefundedEvent(Guid PaymentId, Guid OrderId, decimal Amount, DateTime RefundedDate);

// External events that PaymentService consumes
public record OrderConfirmedEvent(Guid OrderId, Guid UserId, decimal TotalAmount, DateTime ConfirmedDate);

public record OrderCancelledEvent(Guid OrderId, Guid UserId, DateTime CancelledDate);
