namespace Shared.Contracts;

/// <summary>
/// Event published by orchestrator when order saga completes successfully
/// </summary>
public interface IOrderConfirmedEvent
{
    Guid OrderId { get; }
    DateTime ConfirmedDate { get; }
}
