namespace Shared.Contracts;

/// <summary>
/// Event published by PaymentService when payment fails
/// </summary>
public interface IPaymentFailedEvent
{
    Guid OrderId { get; }
    string Reason { get; }
    DateTime FailedDate { get; }
}
