using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Queries;

public record GetOrderByIdQuery(Guid OrderId) 
    : IRequest<OrderDto?>;

public record GetAllOrdersQuery() 
    : IRequest<List<OrderDto>>;

public record GetOrdersByUserIdQuery(Guid UserId) 
    : IRequest<List<OrderDto>>;

public record GetOrdersByStatusQuery(OrderStatus Status) 
    : IRequest<List<OrderDto>>;
