namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime? DeliveredDate { get; private set; }
    public string ShippingAddress { get; private set; } = string.Empty;
    
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    // Constructor for EF Core
    private Order() { }

    public Order(Guid userId, string shippingAddress)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
        OrderDate = DateTime.UtcNow;
    }

    public void AddOrderItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot add items to an order that is not pending");

        var orderItem = new OrderItem(Id, productId, productName, quantity, unitPrice);
        _orderItems.Add(orderItem);
        RecalculateTotal();
    }

    public void RemoveOrderItem(Guid orderItemId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from an order that is not pending");

        var item = _orderItems.FirstOrDefault(i => i.Id == orderItemId);
        if (item != null)
        {
            _orderItems.Remove(item);
            RecalculateTotal();
        }
    }

    private void RecalculateTotal()
    {
        TotalAmount = _orderItems.Sum(i => i.TotalPrice);
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        if (_orderItems.Count == 0)
            throw new InvalidOperationException("Cannot confirm an order with no items");

        Status = OrderStatus.Confirmed;
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be marked as paid");

        Status = OrderStatus.Paid;
    }

    public void MarkAsShipped()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException("Only paid orders can be shipped");

        Status = OrderStatus.Shipped;
        ShippedDate = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered");

        Status = OrderStatus.Delivered;
        DeliveredDate = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel a delivered order");

        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel a shipped order");

        Status = OrderStatus.Cancelled;
    }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Paid,
    Shipped,
    Delivered,
    Cancelled
}
