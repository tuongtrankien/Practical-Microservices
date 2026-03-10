namespace Shared.Contracts;

/// <summary>
/// Event published by orchestrator to request payment refund (compensation)
/// </summary>
public interface IRefundRequestedEvent
{
    Guid OrderId { get; }
    string Reason { get; }
    DateTime RequestedDate { get; }
}
