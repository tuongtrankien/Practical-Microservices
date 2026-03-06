using MassTransit;
using MediatR;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Events;

namespace ProductService.Application.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateProductCommandHandler(
        IProductRepository productRepository, 
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            request.Name, 
            request.Description, 
            request.Price, 
            request.StockQuantity, 
            request.Category);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event to RabbitMQ
        await _publishEndpoint.Publish(new ProductCreatedEvent(
            product.Id, 
            product.Name, 
            product.Price, 
            product.CreatedAt), 
            cancellationToken);

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

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateProductCommandHandler(
        IProductRepository productRepository, 
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found");

        product.UpdateDetails(request.Name, request.Description, request.Price, request.Category);
        
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish update event
        await _publishEndpoint.Publish(new ProductUpdatedEvent(
            product.Id, 
            product.Name, 
            product.Price, 
            product.UpdatedAt ?? DateTime.UtcNow), 
            cancellationToken);

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

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateStockCommandHandler(
        IProductRepository productRepository, 
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ProductDto> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found");

        product.UpdateStock(request.Quantity);
        
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish stock update event
        await _publishEndpoint.Publish(new StockUpdatedEvent(
            product.Id, 
            product.StockQuantity, 
            product.UpdatedAt ?? DateTime.UtcNow), 
            cancellationToken);

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

public class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeactivateProductCommandHandler(
        IProductRepository productRepository, 
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found");

        product.Deactivate();
        
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish deactivation event
        await _publishEndpoint.Publish(new ProductDeactivatedEvent(
            product.Id, 
            DateTime.UtcNow), 
            cancellationToken);

        return true;
    }
}
