namespace Shared.Contracts;

/// <summary>
/// Event published by orchestrator when order saga fails and needs cancellation
/// </summary>
public interface IOrderCancelledEvent
{
    Guid OrderId { get; }
    string Reason { get; }
    DateTime CancelledDate { get; }
}
