using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Commands;
using PaymentService.Application.DTOs;
using PaymentService.Application.Queries;
using PaymentService.Domain.Entities;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<PaymentDto>>> GetAllPayments()
    {
        var payments = await _mediator.Send(new GetAllPaymentsQuery());
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentById(Guid id)
    {
        var payment = await _mediator.Send(new GetPaymentByIdQuery(id));
        
        if (payment == null)
            return NotFound(new { Message = $"Payment with ID {id} not found" });

        return Ok(payment);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentByOrderId(Guid orderId)
    {
        var payment = await _mediator.Send(new GetPaymentByOrderIdQuery(orderId));
        
        if (payment == null)
            return NotFound(new { Message = $"Payment for Order {orderId} not found" });

        return Ok(payment);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<PaymentDto>>> GetPaymentsByUserId(Guid userId)
    {
        var payments = await _mediator.Send(new GetPaymentsByUserIdQuery(userId));
        return Ok(payments);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<PaymentDto>>> GetPaymentsByStatus(PaymentStatus status)
    {
        var payments = await _mediator.Send(new GetPaymentsByStatusQuery(status));
        return Ok(payments);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto dto)
    {
        var command = new CreatePaymentCommand(dto.OrderId, dto.UserId, dto.Amount, dto.PaymentMethod);
        var payment = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
    }

    [HttpPost("{id}/process")]
    public async Task<ActionResult<PaymentDto>> ProcessPayment(Guid id, [FromBody] ProcessPaymentDto dto)
    {
        var payment = await _mediator.Send(new ProcessPaymentCommand(id, dto.TransactionId));
        return Ok(payment);
    }

    [HttpPost("{id}/fail")]
    public async Task<ActionResult<PaymentDto>> FailPayment(Guid id, [FromBody] string reason)
    {
        var payment = await _mediator.Send(new FailPaymentCommand(id, reason));
        return Ok(payment);
    }

    [HttpPost("{id}/refund")]
    public async Task<ActionResult<PaymentDto>> RefundPayment(Guid id)
    {
        var payment = await _mediator.Send(new RefundPaymentCommand(id));
        return Ok(payment);
    }
}
