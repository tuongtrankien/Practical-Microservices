namespace Shared.Contracts;

/// <summary>
/// Event published by orchestrator to request stock reservation for a single product
/// Orchestrator publishes one event per order item
/// </summary>
public interface IStockReservationRequestedEvent
{
    Guid OrderId { get; }
    Guid ProductId { get; }
    int Quantity { get; }
    DateTime RequestedDate { get; }
}
