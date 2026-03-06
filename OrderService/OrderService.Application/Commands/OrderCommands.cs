using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands;

public record CreateOrderCommand(Guid UserId, string ShippingAddress, List<CreateOrderItemDto> OrderItems) 
    : IRequest<OrderDto>;

public record ConfirmOrderCommand(Guid OrderId) 
    : IRequest<OrderDto>;

public record ShipOrderCommand(Guid OrderId) 
    : IRequest<OrderDto>;

public record DeliverOrderCommand(Guid OrderId) 
    : IRequest<OrderDto>;

public record CancelOrderCommand(Guid OrderId) 
    : IRequest<bool>;
