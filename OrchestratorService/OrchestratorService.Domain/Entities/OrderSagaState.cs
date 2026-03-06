namespace OrchestratorService.Domain.Entities;

public class OrderSagaState
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public SagaStatus Status { get; private set; }
    public string CurrentStep { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public decimal OrderAmount { get; private set; }

    // Constructor for EF Core
    private OrderSagaState() { }

    public OrderSagaState(Guid orderId, Guid userId, decimal orderAmount)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        UserId = userId;
        OrderAmount = orderAmount;
        Status = SagaStatus.Started;
        CurrentStep = "OrderPlaced";
        CreatedAt = DateTime.UtcNow;
    }

    public void MoveToStockReservation()
    {
        if (Status != SagaStatus.Started)
            throw new InvalidOperationException($"Cannot move to stock reservation from status {Status}");

        CurrentStep = "StockReservationRequested";
        Status = SagaStatus.StockReservationPending;
    }

    public void CompleteStockReservation()
    {
        if (Status != SagaStatus.StockReservationPending)
            throw new InvalidOperationException($"Cannot complete stock reservation from status {Status}");

        CurrentStep = "StockReserved";
        Status = SagaStatus.StockReserved;
    }

    public void ConfirmOrder()
    {
        if (Status != SagaStatus.StockReserved)
            throw new InvalidOperationException($"Cannot confirm order from status {Status}");

        CurrentStep = "OrderConfirmed";
        Status = SagaStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void FailStockReservation(string errorMessage)
    {
        if (Status != SagaStatus.StockReservationPending)
            throw new InvalidOperationException($"Cannot fail stock reservation from status {Status}");

        CurrentStep = "StockReservationFailed";
        Status = SagaStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }

    public void CancelOrder(string reason)
    {
        CurrentStep = "OrderCancelled";
        Status = SagaStatus.Cancelled;
        ErrorMessage = reason;
        CompletedAt = DateTime.UtcNow;
    }
}

public enum SagaStatus
{
    Started,
    StockReservationPending,
    StockReserved,
    Completed,
    Failed,
    Cancelled
}
