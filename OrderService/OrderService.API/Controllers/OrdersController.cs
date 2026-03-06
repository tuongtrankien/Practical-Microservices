using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Domain.Entities;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
    {
        var orders = await _mediator.Send(new GetAllOrdersQuery());
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        
        if (order == null)
            return NotFound(new { Message = $"Order with ID {id} not found" });

        return Ok(order);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<OrderDto>>> GetOrdersByUserId(Guid userId)
    {
        var orders = await _mediator.Send(new GetOrdersByUserIdQuery(userId));
        return Ok(orders);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<OrderDto>>> GetOrdersByStatus(OrderStatus status)
    {
        var orders = await _mediator.Send(new GetOrdersByStatusQuery(status));
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var command = new CreateOrderCommand(dto.UserId, dto.ShippingAddress, dto.OrderItems);
        var order = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult<OrderDto>> ConfirmOrder(Guid id)
    {
        var order = await _mediator.Send(new ConfirmOrderCommand(id));
        return Ok(order);
    }

    [HttpPost("{id}/ship")]
    public async Task<ActionResult<OrderDto>> ShipOrder(Guid id)
    {
        var order = await _mediator.Send(new ShipOrderCommand(id));
        return Ok(order);
    }

    [HttpPost("{id}/deliver")]
    public async Task<ActionResult<OrderDto>> DeliverOrder(Guid id)
    {
        var order = await _mediator.Send(new DeliverOrderCommand(id));
        return Ok(order);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelOrder(Guid id)
    {
        await _mediator.Send(new CancelOrderCommand(id));
        return NoContent();
    }
}
