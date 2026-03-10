namespace Shared.Contracts;

/// <summary>
/// Event published by PaymentService when payment succeeds
/// </summary>
public interface IPaymentSucceededEvent
{
    Guid OrderId { get; }
    Guid PaymentId { get; }
    decimal Amount { get; }
    DateTime ProcessedDate { get; }
    List<IOrderItemInfo> Items { get; }
}
