namespace ProductService.Domain.Events;

public record ProductCreatedEvent(Guid ProductId, string Name, decimal Price, DateTime CreatedAt);

public record ProductUpdatedEvent(Guid ProductId, string Name, decimal Price, DateTime UpdatedAt);

public record StockUpdatedEvent(Guid ProductId, int NewStockQuantity, DateTime UpdatedAt);

public record ProductDeactivatedEvent(Guid ProductId, DateTime DeactivatedAt);
