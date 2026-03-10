namespace Shared.Contracts;

/// <summary>
/// Event published when an order is placed and needs saga orchestration
/// </summary>
public interface IOrderPlacedEvent
{
    Guid OrderId { get; }
    Guid UserId { get; }
    decimal TotalAmount { get; }
    List<IOrderItemInfo> Items { get; }
    DateTime PlacedDate { get; }
}
