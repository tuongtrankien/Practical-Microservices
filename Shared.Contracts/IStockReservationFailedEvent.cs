namespace Shared.Contracts;

/// <summary>
/// Event published by ProductService when stock reservation fails
/// </summary>
public interface IStockReservationFailedEvent
{
    Guid OrderId { get; }
    Guid ProductId { get; }
    string Reason { get; }
    DateTime FailedDate { get; }
}
