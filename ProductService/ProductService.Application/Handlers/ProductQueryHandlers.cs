using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Application.Queries;

namespace ProductService.Application.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        
        if (product == null)
            return null;

        return new ProductDto(
            product.Id, 
            product.Name, 
            product.Description, 
            product.Price, 
            product.StockQuantity, 
            product.Category, 
            product.CreatedAt, 
            product.IsActive);
    }
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        
        return products.Select(p => new ProductDto(
            p.Id, 
            p.Name, 
            p.Description, 
            p.Price, 
            p.StockQuantity, 
            p.Category, 
            p.CreatedAt, 
            p.IsActive)).ToList();
    }
}

public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsByCategoryQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetByCategoryAsync(request.Category, cancellationToken);
        
        return products.Select(p => new ProductDto(
            p.Id, 
            p.Name, 
            p.Description, 
            p.Price, 
            p.StockQuantity, 
            p.Category, 
            p.CreatedAt, 
            p.IsActive)).ToList();
    }
}
