# Orchestrator Service - Saga Pattern (Stateless)

## Overview
The Orchestrator Service coordinates distributed transactions across microservices using the **Saga Orchestration Pattern**. This is a **lightweight, stateless background service** that orchestrates the complete order flow: **Payment → Stock → Confirmation**.

## Complete Flow Diagram

```
┌─────────────┐
│OrderService │ (1) Creates Order
└──────┬──────┘
       │ OrderPlacedEvent
       ▼
┌─────────────────┐
│  ORCHESTRATOR   │ (2) Requests Payment
└──────┬──────────┘
       │ PaymentRequestedEvent
       ▼
┌─────────────┐
│PaymentSvc   │ (3) Processes Payment
└──────┬──────┘
       │
       ├─── SUCCESS: PaymentSucceededEvent ──┐
       │                                      ├──► NotificationService (Payment Confirmed Email/SMS) 📧
       │                                      ▼
       │                              ┌─────────────────┐
       │                              │  ORCHESTRATOR   │ (4) Requests Stock
       │                              └──────┬──────────┘
       │                                     │ StockReservationRequestedEvent
       │                                     ▼
       │                              ┌─────────────┐
       │                              │ ProductSvc  │ (5) Reserves Stock
       │                              └──────┬──────┘
       │                                     │
       │                                     ├─── SUCCESS: StockReservedCompletedEvent ──┐
       │                                     │                                            ▼
       │                                     │                                    ┌─────────────────┐
       │                                     │                                    │  ORCHESTRATOR   │ (6) Confirms Order
       │                                     │                                    └──────┬──────────┘
       │                                     │                                           │ OrderConfirmedEvent
       │                                     │                                           ▼
       │                                     │                                    ┌─────────────────────┐
       │                                     │                                    │ OrderService +      │
       │                                     │                                    │ NotificationService │ 📧 Order Complete Email/SMS
       │                                     │                                    └─────────────────────┘
       │                                     │                                    ✅ ORDER COMPLETE
       │                                     │
       │                                     └─── FAIL: StockReservationFailedEvent ──┐
       │                                                                               ▼
       │                                                                        ┌─────────────────┐
       │                                                                        │  ORCHESTRATOR   │ (7) Compensation
       │                                                                        └──────┬──────────┘
       │                                                                               │ RefundRequestedEvent
       │                                                                               │ OrderCancelledEvent
       │                                                                               ▼
       │                                                                        ┌─────────────────────┐
       │                                                                        │ PaymentService +    │
       │                                                                        │ OrderService +      │
       │                                                                        │ NotificationService │ 📧 Cancellation Email/SMS
       │                                                                        └─────────────────────┘
       │                                                                        ❌ ORDER CANCELLED (Stock Failed)
       │
       └─── FAIL: PaymentFailedEvent ──┐
                                        ▼
                                 ┌─────────────────┐
                                 │  ORCHESTRATOR   │ (3b) Cancels Order
                                 └──────┬──────────┘
                                        │ OrderCancelledEvent
                                        ▼
                                 ┌─────────────────────┐
                                 │ OrderService +      │
                                 │ NotificationService │ 📧 Payment Failed Email/SMS
                                 └─────────────────────┘
                                 ❌ ORDER CANCELLED (Payment Failed)
```

## Architecture
- **Pattern**: Saga Orchestration (Stateless)
- **Messaging**: MassTransit + RabbitMQ
- **Type**: Background Service (No REST API, No Database)
- **Purpose**: Event-driven workflow coordination

## Why Stateless?
Unlike the typical Clean Architecture microservices in this solution, the Orchestrator doesn't need:
- ❌ Database (no state persistence)
- ❌ Clean Architecture layers (Domain, Application)
- ❌ Repositories or Unit of Work
- ❌ REST API controllers

It simply:
- ✅ Listens to events from other services
- ✅ Applies orchestration logic
- ✅ Publishes next events in the workflow

## Saga Flow

### Success Path
```
OrderService → OrderPlacedEvent
    ↓
Orchestrator → PaymentRequestedEvent
    ↓
PaymentService → PaymentSucceededEvent
    ↓
    ├──► NotificationService (Payment confirmed 📧)
    └──► Orchestrator → StockReservationRequestedEvent
             ↓
ProductService → StockReservedCompletedEvent
    ↓
Orchestrator → OrderConfirmedEvent
    ↓
OrderService + NotificationService (Order complete 📧)
```

### Failure Path 1: Payment Fails
```
OrderService → OrderPlacedEvent
    ↓
Orchestrator → PaymentRequestedEvent
    ↓
PaymentService → PaymentFailedEvent
    ↓
Orchestrator → OrderCancelledEvent
    ↓ (Payment failed 📧)
OrderService + NotificationService
```

### Failure Path 2: Stock Reservation Fails (with Compensation)
```
OrderService → OrderPlacedEvent
    ↓
Orchestrator → PaymentRequestedEvent
    ↓
PaymentService → PaymentSucceededEvent
    ↓
    ├──► NotificationService (Payment confirmed 📧)
    └──► Orchestrator → StockReservationRequestedEvent
             ↓
ProductService → StockReservationFailedEvent
    ↓
Orchestrator → RefundRequestedEvent + OrderCancelledEvent
    ↓
PaymentService (Refund) + OrderService + NotificationService (Stock unavailable 📧)
```

## Project Structure
```
OrchestratorService.API/
├── Program.cs                    # Minimal hosting (health check only)
├── appsettings.json              # RabbitMQ configuration

OrchestratorService.Infrastructure/
├── Consumers/
│   ├── OrderPlacedConsumer.cs              # Step 1: Consume order → Request payment
│   ├── PaymentSucceededConsumer.cs         # Step 2: Payment OK → Request stock
│   ├── PaymentFailedConsumer.cs            # Step 2 (Compensation): Payment failed → Cancel order
│   ├── StockReservedCompletedConsumer.cs   # Step 3: Stock OK → Confirm order
│   └── StockReservationFailedConsumer.cs   # Step 3 (Compensation): Stock failed → Refund + Cancel
├── Events/
│   └── SagaEvents.cs                       # All event contracts
└── DependencyInjection.cs                  # MassTransit configuration
```

## Events

### PaymentSucceededEvent**: From PaymentService when payment is processed successfully
- **PaymentFailedEvent**: From PaymentService when payment processing fails
- **StockReservedCompletedEvent**: From ProductService when stock is successfully reserved
- **StockReservationFailedEvent**: From ProductService when stock reservation fails

### Published Events
- **PaymentRequestedEvent**: To PaymentService to process payment
- **StockReservationRequestedEvent**: To ProductService to reserve stock
- **RefundRequestedEvent**: To PaymentService to refund payment (compensation)
### Published Events
- **StockReservationRequestedEvent**: To ProductService to reserve stock
- **OrderConfirmedEvent**: To OrderService + NotificationService when saga succeeds
- **OrderCancelledEvent**: To OrderService + NotificationService when saga fails

## Configuration

### appsettings.json
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest"
  }
}
```payment-succeeded-queue`: Receives PaymentSucceededEvent
- `orchestrator-payment-failed-queue`: Receives PaymentFailedEvent
- `orchestrator-

### Queue Endpoints
- `orchestrator-order-placed-queue`: Receives OrderPlacedEvent
- `orchestrator-stock-reserved-queue`: Receives StockReservedCompletedEvent
- `orchestrator-stock-failed-queue`: Receives StockReservationFailedEvent

## Running the Service

### Prerequisites
- .NET 8 SDK
- RabbitMQ running on localhost:5672

### Start the service
```powershell
cd OrchestratorService/OrchestratorService.API
dotnet run
```

### Health Check
```
GET http://localhost:5000/health
GET http://localhost:5000/
```

## Integration with Other Services

### OrderService
- **Publishes**: `OrderPlacedEvent` when order is created
- **Consumes**: `OrderConfirmedEvent` (update order status to Confirmed)
- **CaymentService
- **Consumes**: `PaymentRequestedEvent` (process payment)
- **Publishes**: `PaymentSucceededEvent` (payment succeeded - includes order items)
- **Publishes**: `PaymentFailedEvent` (payment failed - insufficient funds, invalid card, etc.)
- **Consumes**: `RefundRequestedEvent` (refund payment when order is cancelled after payment)

### ProductService
- **Consumes**: `StockReservationRequestedEvent` (reserve stock)
- **Publishes**: `StockReservedCompletedEvent` (stock successfully reserved)
- **Publishes**: `StockReservationFailedEvent` (stock)
- **Publishes**: `StockReservedCompletedEvent` (success)
- **Publishes**: `StockReservationFailedEvent` (failure - insufficient stock)

### NotificationService
- **Consumes**: `PaymentSucceededEvent` (send "Payment confirmed" email/SMS)
- **Consumes**: `OrderConfirmedEvent` (send "Order confirmed, preparing shipment" email/SMS)
- **Consumes**: `OrderCancelledEvent` (send "Order cancelled" email/SMS with reason)

## Retry Policy
All consumers have retry configured:
- **Retry Count**: 3 attempts
- **Retry Interval**: 5 seconds between attempts

## Logging
The service outputs orchestration progress to console:
- 🎯 OrderPlacedEvent received
- 📤 Published PaymentRequestedEvent
- ✅ PaymentSucceededEvent received → **NotificationService notified**
- 📤 Published StockReservationRequestedEvent
- ✅ StockReservedCompletedEvent received
- 🎉 Saga completed successfully → **NotificationService notified**
- ⚠️ PaymentFailedEvent received → Order cancelled → **NotificationService notified**
- ⚠️ StockReservationFailedEvent received → Refund requested → Order cancelled → **NotificationService notified**

## Design Decisions

### Why No Database?
- **Idempotency**: Handled by MassTransit message deduplication
- **Saga State**: Events carry necessary context (e.g., PaymentSucceededEvent includes order items)
- **Scalability**: Stateless services scale horizontally easily
- **Simplicity**: No database migrations, no ORM overhead

### Trade-off: Stateless Design
In this stateless design, **PaymentSucceededEvent must include order items** so the orchestrator can proceed with stock reservation. This means:
- ✅ PaymentService receives order items in PaymentRequestedEvent
- ✅ PaymentService echoes order items back in PaymentSucceededEvent
- ✅ Orchestrator has all needed data without querying databases

### Notification Strategy
**NotificationService** is a pure consumer that listens to multiple events to send notifications at different stages:
1. **PaymentSucceededEvent** → "Your payment of $X has been confirmed" 📧
2. **OrderConfirmedEvent** → "Your order is confirmed and will be shipped soon" 📧
3. **OrderCancelledEvent** → "Your order has been cancelled. Reason: {reason}" 📧

This decoupled approach allows NotificationService to independently decide what to notify without the orchestrator needing to know about notifications.

### When to Use Stateful Saga?
Consider a stateful saga with database persistence if you need:
- Long-running sagas (hours/days)
- Complex multi-step workflows with many compensations
- Audit trail of saga transitions
- Ability to query saga state

For simple, short-lived orchestrations like order placement, stateless is sufficient.

## Future Enhancements
- Add shipping service to the saga (after stock reservation)
- Implement correlation ID tracking across all events
- Add distributed tracing (OpenTelemetry)
- Add metrics and monitoring (Prometheus)
- Handle partial stock reservation (some items available, some not)
- Implement timeout handling (e.g., payment takes too long)
