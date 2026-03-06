using OrderService.Domain.Entities;

namespace OrderService.Application.DTOs;

public record OrderDto(
    Guid Id,
    Guid UserId,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime OrderDate,
    DateTime? ShippedDate,
    DateTime? DeliveredDate,
    string ShippingAddress,
    List<OrderItemDto> OrderItems);

public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

public record CreateOrderDto(
    Guid UserId,
    string ShippingAddress,
    List<CreateOrderItemDto> OrderItems);

public record CreateOrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);

public record UpdateOrderStatusDto(
    OrderStatus Status);
