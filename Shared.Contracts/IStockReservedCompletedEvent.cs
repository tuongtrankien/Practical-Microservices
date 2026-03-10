namespace Shared.Contracts;

/// <summary>
/// Event published by ProductService when stock reservation succeeds
/// </summary>
public interface IStockReservedCompletedEvent
{
    Guid OrderId { get; }
    Guid ProductId { get; }
    int Quantity { get; }
    DateTime ReservedDate { get; }
}
