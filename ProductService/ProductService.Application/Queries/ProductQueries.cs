using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries;

public record GetProductByIdQuery(Guid ProductId) 
    : IRequest<ProductDto?>;

public record GetAllProductsQuery() 
    : IRequest<List<ProductDto>>;

public record GetProductsByCategoryQuery(string Category) 
    : IRequest<List<ProductDto>>;
