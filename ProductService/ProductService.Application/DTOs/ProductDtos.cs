namespace ProductService.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string Category,
    DateTime CreatedAt,
    bool IsActive
);

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string Category
);

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    string Category
);

public record UpdateStockDto(
    int Quantity
);
