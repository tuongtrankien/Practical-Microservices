# eCommerce Microservices - Implementation Guide

## ✅ Completed Configuration

### Architecture Decisions
- **Database**: Each microservice has its own SQL Server database  (Database per Service pattern)
- **CQRS**: Implemented using MediatR for command/query separation
- **Event-Driven**: RabbitMQ with MassTransit for inter-service communication
- **Clean Architecture**: Strict layer separation with proper dependency flow

## 🏗️ Microservices Architecture

### 1. UserService ✅ FULLY IMPLEMENTED
**Responsibility**: User authentication, authorization, and profile management

**Database**: `UserServiceDb`
- Entities: User
- Features: Register, Update, Activate/Deactivate

**Events**: None (standalone service, no event publishing)

**API Endpoints**:
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/email/{email}` - Get user by email
- `POST /api/users` - Create user
- `PUT /api/users/{id}` - Update user
- `POST /api/users/{id}/deactivate` - Deactivate user
- `POST /api/users/{id}/activate` - Activate user

**Connection String**: 
```
Server=localhost;Database=UserServiceDb;Trusted_Connection=True;TrustServerCertificate=True;
```

**Note**: UserService does NOT use RabbitMQ - it's a standalone service focused on user management only.

---

### 2. ProductService 🔧 TO IMPLEMENT
**Responsibility**: Product catalog, inventory management

**Database**: `ProductServiceDb`

**Entities**:
```csharp
public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }
}
```

**Events to Publish**:
- `ProductCreatedEvent`
- `ProductUpdatedEvent`
- `StockUpdatedEvent`
- `ProductDeactivatedEvent`

**CQRS Commands**:
- `CreateProductCommand`
- `UpdateProductCommand`
- `UpdateStockCommand`
- `DeactivateProductCommand`

**CQRS Queries**:
- `GetProductByIdQuery`
- `GetAllProductsQuery`
- `GetProductsByCategoryQuery`

**Connection String**: 
```
Server=localhost;Database=ProductServiceDb;Trusted_Connection=True;TrustServerCertificate=True;
```

---

### 3. OrderService 🔧 TO IMPLEMENT
**Responsibility**: Order processing, order lifecycle management

**Database**: `OrderServiceDb`

**Entities**:
```csharp
public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; } // Pending, Confirmed, Shipped, Delivered, Cancelled
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; }
}

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
}
```

**Events to Publish**:
- `OrderCreatedEvent`
- `OrderConfirmedEvent`
- `OrderShippedEvent`
- `OrderDeliveredEvent`
- `OrderCancelledEvent`

**Events to Consume**:
- `PaymentCompletedEvent` (from PaymentService)
- `PaymentFailedEvent` (from PaymentService)

**CQRS Commands**:
- `CreateOrderCommand`
- `ConfirmOrderCommand`
- `ShipOrderCommand`
- `DeliverOrderCommand`
- `CancelOrderCommand`

**CQRS Queries**:
- `GetOrderByIdQuery`
- `GetOrdersByUserIdQuery`
- `GetAllOrdersQuery`

**Connection String**: 
```
Server=localhost;Database=OrderServiceDb;Trusted_Connection=True;TrustServerCertificate=True;
```

---

### 4. PaymentService 🔧 TO IMPLEMENT
**Responsibility**: Payment processing, refunds, payment methods

**Database**: `PaymentServiceDb`

**Entities**:
```csharp
public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; } // Pending, Completed, Failed, Refunded
    public PaymentMethod Method { get; private set; } // CreditCard, DebitCard, PayPal, BankTransfer
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
}
```

**Events to Publish**:
- `PaymentInitiatedEvent`
- `PaymentCompletedEvent`
- `PaymentFailedEvent`
- `PaymentRefundedEvent`

**Events to Consume**:
- `OrderCreatedEvent` (from OrderService)
- `OrderCancelledEvent` (from OrderService)

**CQRS Commands**:
- `ProcessPaymentCommand`
- `RefundPaymentCommand`

**CQRS Queries**:
- `GetPaymentByIdQuery`
- `GetPaymentsByOrderIdQuery`
- `GetPaymentsByUserIdQuery`

**Connection String**: 
```
Server=localhost;Database=PaymentServiceDb;Trusted_Connection=True;TrustServerCertificate=True;
```

---

### 5. NotificationService 🔧 TO IMPLEMENT (Event Consumer Only)
**Responsibility**: Send email, SMS, and push notifications

**Database**: NONE (or simple logging database)

**No CQRS Commands/Queries** - This is a pure event consumer

**Events to Consume**:
- `OrderCreatedEvent` (from OrderService) → Send order confirmation
- `OrderShippedEvent` (from OrderService) → Send shipping notification
- `PaymentCompletedEvent` (from PaymentService) → Send payment receipt
- `PaymentFailedEvent` (from PaymentService) → Send payment failure notification
- `ProductCreatedEvent` (from ProductService) → Notify relevant parties (optional)

**Event Consumers** (MassTransit):
```csharp
public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        // Send order confirmation email
        await SendEmail(context.Message.UserEmail, "Order Confirmation");
    }
}
```

**RabbitMQ Configuration Only** - No database needed:
```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<OrderShippedEventConsumer>();
    x.AddConsumer<PaymentCompletedEventConsumer>();
    x.AddConsumer<PaymentFailedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});
```

---

## 📁 Project Structure (Applied to All Services)

```
<ServiceName>/
├── <ServiceName>.Domain/
│   ├── Entities/
│   │   └── <Entity>.cs
│   ├── Events/
│   │   └── <Event>Events.cs
│   └── Exceptions/
│       └── DomainExceptions.cs
│
├── <ServiceName>.Application/
│   ├── Commands/
│   │   └── <Entity>Commands.cs
│   ├── Queries/
│   │   └── <Entity>Queries.cs
│   ├── Handlers/
│   │   ├── <Entity>CommandHandlers.cs
│   │   └── <Entity>QueryHandlers.cs
│   ├── DTOs/
│   │   └── <Entity>Dtos.cs
│   ├── Interfaces/
│   │   └── IRepositories.cs
│   └── DependencyInjection.cs
│
├── <ServiceName>.Infrastructure/
│   ├── Data/
│   │   ├── <Service>DbContext.cs
│   │   ├── Configurations/
│   │   │   └── <Entity>Configuration.cs
│   │   └── Repositories/
│   │       └── <Entity>Repository.cs
│   ├── Consumers/ (for event consumers)
│   │   └── <Event>Consumer.cs
│   └── DependencyInjection.cs
│
├── <ServiceName>.API/
│   ├── Controllers/
│   │   └── <Entity>Controller.cs
│   ├── Program.cs
│   └── appsettings.json
│
└── <ServiceName>.Tests/
    ├── Unit/
    ├── Integration/
    └── Functional/
```

---

## 🔧 NuGet Packages Installed

### Application Layer:
- MediatR (9.0.1)
- FluentValidation
- AutoMapper
- MassTransit (9.0.1)
- BCrypt.Net-Next (for UserService)

### Infrastructure Layer:
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design
- MassTransit.RabbitMQ (9.0.1)

---

## ⚙️ Configuration Files

### appsettings.json Pattern (All Services except NotificationService):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=<ServiceName>Db;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### NotificationService appsettings.json:
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@ecommerce.com",
    "SenderName": "eCommerce System"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## 🚀 Running the Microservices

### Prerequisites:
1. .NET 8 SDK installed
2. SQL Server running (localhost)
3. RabbitMQ running (localhost)

### Run RabbitMQ (Docker):
```bash
docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### Run SQL Server (Docker):
```bash
docker run -d --name sqlserver -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

### Create Migrations (for each service with DB):
```bash
cd <ServiceName>
dotnet ef migrations add InitialCreate --project <ServiceName>.Infrastructure --startup-project <ServiceName>.API
dotnet ef database update --project <ServiceName>.Infrastructure --startup-project <ServiceName>.API
```

### Run Services:
```bash
# Terminal 1 - UserService
cd UserService/UserService.API
dotnet run

# Terminal 2 - ProductService
cd ProductService/ProductService.API
dotnet run

# Terminal 3 - OrderService
cd OrderService/OrderService.API
dotnet run

# Terminal 4 - PaymentService
cd PaymentService/PaymentService.API
dotnet run

# Terminal 5 - NotificationService
cd NotificationService/NotificationService.API
dotnet run
```

### Default Ports (configure in launchSettings.json):
- UserService: https://localhost:5001
- ProductService: https://localhost:5002
- OrderService: https://localhost:5003
- PaymentService: https://localhost:5004
- NotificationService: https://localhost:5005

---

## 📊 Event Flow Example

### User Registration Flow:
1. **Client** → POST /api/users (UserService)
2. **UserService** → Creates User in DB
3. **UserService** → Returns user data
**Note**: UserService is standalone - no events published

### Order Creation Flow:
1. **Client** → POST /api/orders (OrderService)
2. **OrderService** → Creates Order in DB (Status: Pending)
3. **OrderService** → Publishes `OrderCreatedEvent` to RabbitMQ
4. **PaymentService** → Consumes `OrderCreatedEvent`
5. **PaymentService** → Processes payment
6. **PaymentService** → Publishes `PaymentCompletedEvent`
7. **OrderService** → Consumes `PaymentCompletedEvent`
8. **OrderService** → Updates Order Status to Confirmed
9. **NotificationService** → Consumes both events → Sends emails

---

## 🧪 Testing

### Test User Creation (UserService):
```bash
curl -X POST https://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePassword123"
  }'
```

### Check RabbitMQ Management UI:
- URL: http://localhost:15672
- Username: guest
- Password: guest
- Check Queues and Exchanges to see messages

---

## 📝 Next Steps

### To Complete ProductService:
1. Copy User Service files as templates
2. Replace User entity with Product entity
3. Implement product-specific business logic
4. Create product migrations
5. Test CRUD operations

### To Complete OrderService:
1. Implement Order and OrderItem entities
2. Create order state machine (Pending → Confirmed → Shipped → Delivered)
3. Add event consumers for Payment events
4. Implement order cancellation logic

### To Complete PaymentService:
1. Implement Payment entity
2. Create payment processing logic (can be simulated)
3. Add event consumers for Order events
4. Implement refund logic

### To Complete NotificationService:
1. Remove Domain and Application layers (not needed)
2. Create event consumers for Order and Payment events
3. Implement email/SMS sending services
4. Add notification templates
5. Configure RabbitMQ to consume events from ProductService, OrderService, and PaymentService

---

## 🏆 Benefits of This Architecture

✅ **Decentralized Databases**: Each service owns its data  
✅ **Loose Coupling**: Services communicate via events  
✅ **CQRS**: Clear separation of reads/writes  
✅ **Scalability**: Each service can scale independently  
✅ **Maintainability**: Clean Architecture keeps code organized  
✅ **Testability**: Each layer can be tested independently  
✅ **Event-Driven**: Asynchronous communication  
✅ **Domain-Driven**: Business logic in Domain layer  

---

## 📚 References

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [MassTransit Documentation](https://masstransit-project.com/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
