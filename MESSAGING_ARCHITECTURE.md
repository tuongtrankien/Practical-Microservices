# Microservices Messaging Architecture

## 🎯 RabbitMQ/Event-Driven Services

The following services use RabbitMQ for inter-service communication:

### 1. **ProductService** 📦
**Role**: Event Publisher

**Events Published**:
- `ProductCreatedEvent` - When a new product is added
- `ProductUpdatedEvent` - When product details change
- `StockUpdatedEvent` - When stock quantity changes
- `ProductDeactivatedEvent` - When a product is removed

**RabbitMQ**: ✅ Required (publish events)

---

### 2. **OrderService** 📋
**Role**: Event Publisher & Consumer

**Events Published**:
- `OrderCreatedEvent` - When a new order is placed
- `OrderConfirmedEvent` - When order is confirmed after payment
- `OrderShippedEvent` - When order is shipped
- `OrderDeliveredEvent` - When order is delivered
- `OrderCancelledEvent` - When order is cancelled

**Events Consumed**:
- `PaymentCompletedEvent` (from PaymentService) → Confirm order
- `PaymentFailedEvent` (from PaymentService) → Cancel order

**RabbitMQ**: ✅ Required (publish and consume events)

---

### 3. **PaymentService** 💳
**Role**: Event Publisher & Consumer

**Events Published**:
- `PaymentInitiatedEvent` - When payment processing starts
- `PaymentCompletedEvent` - When payment succeeds
- `PaymentFailedEvent` - When payment fails
- `PaymentRefundedEvent` - When refund is processed

**Events Consumed**:
- `OrderCreatedEvent` (from OrderService) → Process payment
- `OrderCancelledEvent` (from OrderService) → Refund payment

**RabbitMQ**: ✅ Required (publish and consume events)

---

### 4. **NotificationService** 📧
**Role**: Event Consumer Only

**Events Consumed**:
- `OrderCreatedEvent` (from OrderService) → Send order confirmation email
- `OrderShippedEvent` (from OrderService) → Send shipping notification
- `OrderDeliveredEvent` (from OrderService) → Send delivery confirmation
- `PaymentCompletedEvent` (from PaymentService) → Send payment receipt
- `PaymentFailedEvent` (from PaymentService) → Send payment failure alert
- `ProductCreatedEvent` (from ProductService) → Optional admin notifications

**No Events Published** - This is a pure consumer service

**RabbitMQ**: ✅ Required (consume events only)

---

## 🚫 Standalone Service (No RabbitMQ)

### 5. **UserService** 👤
**Role**: Standalone CRUD Service

**Features**:
- User registration
- User profile management
- User authentication (future)
- User activation/deactivation

**No Events Published or Consumed** - This service operates independently

**RabbitMQ**: ❌ NOT Required

**Why?** 
UserService is a foundational service that manages user data. Other services can query UserService directly via HTTP API when they need user information, rather than consuming events. This keeps UserService simple and focused on user management without coupling to the event-driven architecture.

---

## 📊 Event Flow Diagram

```
┌─────────────────┐
│  ProductService │ ──► ProductCreatedEvent ──────────► NotificationService
│   (Publisher)   │ ──► StockUpdatedEvent
└─────────────────┘

┌─────────────────┐
│  OrderService   │ ──► OrderCreatedEvent ────────────► PaymentService
│ (Pub & Consumer)│ ──► OrderShippedEvent ─────────────► NotificationService
└─────────────────┘      ▲
                         │
                  PaymentCompletedEvent
                         │
┌─────────────────┐      │
│ PaymentService  │ ─────┘
│ (Pub & Consumer)│ ──► PaymentCompletedEvent ────────► NotificationService
└─────────────────┘ ──► PaymentFailedEvent ───────────► NotificationService

┌─────────────────┐
│  UserService    │ ──► No Events (HTTP API only)
│  (Standalone)   │
└─────────────────┘
```

---

## 🔧 RabbitMQ Configuration by Service

### ProductService, OrderService, PaymentService
```csharp
// Infrastructure/DependencyInjection.cs
services.AddMassTransit(x =>
{
    // Add consumers if needed (OrderService, PaymentService)
    x.AddConsumer<PaymentCompletedEventConsumer>(); // Example

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});
```

### NotificationService (Consumer Only)
```csharp
// Infrastructure/DependencyInjection.cs
services.AddMassTransit(x =>
{
    // Register all event consumers
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<OrderShippedEventConsumer>();
    x.AddConsumer<PaymentCompletedEventConsumer>();
    x.AddConsumer<PaymentFailedEventConsumer>();
    x.AddConsumer<ProductCreatedEventConsumer>(); // Optional

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});
```

### UserService
```csharp
// Infrastructure/DependencyInjection.cs
// NO RabbitMQ/MassTransit configuration
services.AddDbContext<UserDbContext>(...);
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
```

---

## 🎭 Use Cases

### Use Case 1: Place an Order
1. Client → POST /api/orders (OrderService)
2. OrderService → Creates order → Publishes `OrderCreatedEvent`
3. PaymentService → Consumes event → Processes payment
4. PaymentService → Publishes `PaymentCompletedEvent`
5. OrderService → Consumes event → Updates order status
6. NotificationService → Consumes both events → Sends emails

### Use Case 2: Create a Product
1. Client → POST /api/products (ProductService)
2. ProductService → Creates product → Publishes `ProductCreatedEvent`
3. NotificationService → Consumes event → Sends admin notification (optional)

### Use Case 3: Register a User
1. Client → POST /api/users (UserService)
2. UserService → Creates user → Returns response
3. **No events published** - Direct HTTP response only

---

## 📦 Required Infrastructure

### For UserService Only
- SQL Server

### For ProductService, OrderService, PaymentService, NotificationService
- SQL Server (except NotificationService - optional)
- RabbitMQ server

---

## 🚀 Starting Services

### Start RabbitMQ (for ProductService, OrderService, PaymentService, NotificationService)
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### Start SQL Server (for all services with databases)
```bash
docker run -d --name sqlserver ^
  -e "ACCEPT_EULA=Y" ^
  -e "SA_PASSWORD=YourStrong@Passw0rd" ^
  -p 1433:1433 ^
  mcr.microsoft.com/mssql/server:2022-latest
```

### Service Startup Order
1. **UserService** - Can start independently
2. **ProductService** - Requires RabbitMQ
3. **OrderService** - Requires RabbitMQ
4. **PaymentService** - Requires RabbitMQ
5. **NotificationService** - Requires RabbitMQ, should start last to consume events

---

## ✅ Summary

| Service | Database | RabbitMQ | Publishes Events | Consumes Events |
|---------|----------|----------|------------------|-----------------|
| UserService | ✅ UserServiceDb | ❌ No | ❌ No | ❌ No |
| ProductService | ✅ ProductServiceDb | ✅ Yes | ✅ Yes | ❌ No |
| OrderService | ✅ OrderServiceDb | ✅ Yes | ✅ Yes | ✅ Yes |
| PaymentService | ✅ PaymentServiceDb | ✅ Yes | ✅ Yes | ✅ Yes |
| NotificationService | ❌ No DB | ✅ Yes | ❌ No | ✅ Yes |
