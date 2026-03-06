using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
    {
        var products = await _mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        
        if (product == null)
            return NotFound(new { Message = $"Product with ID {id} not found" });

        return Ok(product);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<ProductDto>>> GetProductsByCategory(string category)
    {
        var products = await _mediator.Send(new GetProductsByCategoryQuery(category));
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto)
    {
        var command = new CreateProductCommand(
            dto.Name, 
            dto.Description, 
            dto.Price, 
            dto.StockQuantity, 
            dto.Category);
            
        var product = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        var command = new UpdateProductCommand(
            id, 
            dto.Name, 
            dto.Description, 
            dto.Price, 
            dto.Category);
            
        var product = await _mediator.Send(command);
        return Ok(product);
    }

    [HttpPatch("{id}/stock")]
    public async Task<ActionResult<ProductDto>> UpdateStock(Guid id, [FromBody] UpdateStockDto dto)
    {
        var command = new UpdateStockCommand(id, dto.Quantity);
        var product = await _mediator.Send(command);
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeactivateProduct(Guid id)
    {
        await _mediator.Send(new DeactivateProductCommand(id));
        return NoContent();
    }
}
