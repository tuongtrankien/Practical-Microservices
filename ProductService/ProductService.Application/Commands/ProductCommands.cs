using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Commands;

public record CreateProductCommand(string Name, string Description, decimal Price, int StockQuantity, string Category) 
    : IRequest<ProductDto>;

public record UpdateProductCommand(Guid ProductId, string Name, string Description, decimal Price, string Category) 
    : IRequest<ProductDto>;

public record UpdateStockCommand(Guid ProductId, int Quantity) 
    : IRequest<ProductDto>;

public record DeactivateProductCommand(Guid ProductId) 
    : IRequest<bool>;
