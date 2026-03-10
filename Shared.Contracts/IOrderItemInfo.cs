namespace Shared.Contracts;

/// <summary>
/// Represents an order item in saga events
/// </summary>
public interface IOrderItemInfo
{
    Guid ProductId { get; }
    int Quantity { get; }
    decimal Price { get; }
}
