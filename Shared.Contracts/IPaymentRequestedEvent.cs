namespace Shared.Contracts;

/// <summary>
/// Event published by orchestrator to request payment processing
/// Includes order items for forwarding to stock reservation step
/// </summary>
public interface IPaymentRequestedEvent
{
    Guid OrderId { get; }
    Guid UserId { get; }
    decimal Amount { get; }
    List<IOrderItemInfo> Items { get; }
    DateTime RequestedDate { get; }
}
