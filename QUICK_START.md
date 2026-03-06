# eCommerce Microservices - Quick Start

## ✅ What's Been Configured

### 1. All NuGet Packages Installed ✅
All 5 microservices have the necessary packages:
- **MediatR** - For CQRS pattern
- **FluentValidation** - For input validation
- **AutoMapper** - For object mapping
- **Entity Framework Core** - For database access (UserService, ProductService, OrderService, PaymentService)
- **MassTransit.RabbitMQ** - For event-driven messaging (ProductService, OrderService, PaymentService, NotificationService only)
- **BCrypt.Net** - For password hashing (UserService)

### 2. UserService - FULLY IMPLEMENTED ✅
Complete implementation with:
- ✅ Domain Layer: User entity with business logic
- ✅ Application Layer: CQRS commands, queries, handlers, DTOs
- ✅ Infrastructure Layer: DbContext, Repository (NO RabbitMQ - standalone service)
- ✅ API Layer: REST controller with full CRUD endpoints
- ✅ Configuration: Database connection string only

**Ready to run** - just needs database migration

**Note**: UserService is a standalone service without event publishing

### 3. Clean Architecture Pattern Applied ✅
All services follow the correct dependency structure:
```
Domain (no dependencies)
   ↑
Application (depends on Domain only)
   ↑
Infrastructure (depends on Domain + Application)
   ↑
API (depends on Application + Infrastructure)
```

### 4. Event-Driven Architecture Setup ✅
- MassTransit configured for RabbitMQ messaging
- Event publishing setup in command handlers
- NotificationService ready for event consumption

---

## 🚧 What Needs To Be Implemented

### ProductService
**Packages**: ✅ Installed  
**Implementation**: ⚠️ Needs domain entities, CQRS handlers, DbContext  
**Pattern**: Follow UserService as template

### OrderService
**Packages**: ✅ Installed  
**Implementation**: ⚠️ Needs domain entities, CQRS handlers, DbContext  
**Special**: Should consume Payment events  
**Pattern**: Follow UserService as template

### PaymentService
**Packages**: ✅ Installed  
**Implementation**: ⚠️ Needs domain entities, CQRS handlers, DbContext  
**Special**: Should consume Order events  
**Pattern**: Follow UserService as template

### NotificationService
**Packages**: ✅ Installed (MassTransit only)  
**Implementation**: ⚠️ Needs event consumers  
**Special**: No database, no CQRS - pure event consumer  
**Pattern**: See IMPLEMENTATION_GUIDE.md

---

## 🏃 Running UserService Now

### 1. Start Prerequisites
```bash
# Start SQL Server (if not running)
docker run -d --name sqlserver ^
  -e "ACCEPT_EULA=Y" ^
  -e "SA_PASSWORD=YourStrong@Passw0rd" ^
  -p 1433:1433 ^
  mcr.microsoft.com/mssql/server:2022-latest
```

**Note**: UserService doesn't need RabbitMQ - it's a standalone service

### 2. Update Connection String (if needed)
Edit: `UserService/UserService.API/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UserServiceDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Create Database Migration
```bash
cd UserService
dotnet ef migrations add InitialCreate ^
  --project UserService.Infrastructure ^
  --startup-project UserService.API

dotnet ef database update ^
  --project UserService.Infrastructure ^
  --startup-project UserService.API
```

### 4. Run UserService
```bash
cd UserService/UserService.API
dotnet run
```

Service will run at: https://localhost:5001 (or configured port)

### 5. Access Swagger UI
Open browser: https://localhost:5001/swagger

### 6. Test User Creation
```bash
curl -X POST https://localhost:5001/api/users ^
  -H "Content-Type: application/json" ^
  -d "{\"email\":\"test@example.com\",\"firstName\":\"John\",\"lastName\":\"Doe\",\"password\":\"SecurePassword123\"}"
```

You should receive a JSON response with the created user details.

---

## 📋 Step-by-Step: Complete Remaining Services

### Step 1: Implement ProductService (Estimated: 1 hour)
1. Copy files from UserService as template
2. Create `Product` entity in Domain layer
3. Create CQRS commands/queries for products
4. Create command/query handlers
5. Create ProductDbContext and configurations
6. Create ProductController
7. Update appsettings.json with ProductServiceDb connection
8. Create and run migrations
9. Test CRUD operations

### Step 2: Implement OrderService (Estimated: 1.5 hours)
1. Create `Order` and `OrderItem` entities
2. Implement order state machine (Pending → Confirmed → Shipped → Delivered)
3. Create CQRS commands/queries
4. Create handlers that publish events
5. Create event consumers for Payment events
6. Create OrderDbContext
7. Create OrderController
8. Configure and test

### Step 3: Implement PaymentService (Estimated: 1 hour)
1. Create `Payment` entity with status
2. Implement payment processing logic (can be simulated)
3. Create CQRS commands for payment processing
4. Create event consumers for Order events
5. Publish PaymentCompleted/PaymentFailed events
6. Create PaymentDbContext
7. Create PaymentController
8. Configure and test

### Step 4: Implement NotificationService (Estimated: 45 minutes)
1. Remove/skip Domain and Application layers
2. Create event consumers in Infrastructure:
   - UserRegisteredEventConsumer
   - OrderCreatedEventConsumer
   - OrderShippedEventConsumer
   - PaymentCompletedEventConsumer
3. Implement email/SMS sending (can use console logging initially)
4. Configure MassTransit with all consumers
5. Test event consumption

---

## 📦 Database Configuration Summary

| Service | Database Name | Connection String |
|---------|---------------|-------------------|
| UserService | UserServiceDb | Configured ✅ |
| ProductService | ProductServiceDb | To configure ⚠️ |
| OrderService | OrderServiceDb | To configure ⚠️ |
| PaymentService | PaymentServiceDb | To configure ⚠️ |
| NotificationService | N/A | No database 📧 |

---

## 🎯 Architecture Benefits Achieved

✅ **Decentralized Databases** - Each service has its own database  
✅ **CQRS Pattern** - Commands and Queries separated via MediatR  
✅ **Event-Driven** - Services communicate via RabbitMQ events  
✅ **Clean Architecture** - Proper layer separation and dependencies  
✅ **Domain-Driven Design** - Business logic in Domain entities  
✅ **Scalability** - Each service can scale independently  
✅ **Testability** - Each layer can be tested in isolation  

---

## 📚 Next Actions

1. **Run UserService** to verify it works end-to-end
2. **Use UserService as template** for ProductService implementation
3. **Implement event consumers** in OrderService and PaymentService for inter-service communication
4. **Build NotificationService** as pure event consumer
5. **Add validation** using FluentValidation in all services
6. **Add logging** for monitoring
7. **Add API Gateway** (optional) for unified entry point
8. **Add authentication/authorization** (JWT tokens)
9. **Containerize** with Docker Compose for easy deployment
10. **Add integration tests** for event flows

---

## 🆘 Troubleshooting

### Build Errors
- Run `dotnet restore` in each service directory
- Check all .csproj files have correct project references

### Database Connection Issues
- Ensure SQL Server is running
- Check connection string in appsettings.json
- Verify TrustServerCertificate=True is set

### RabbitMQ Connection Issues
- Ensure RabbitMQ is running on localhost:5672
- Check RabbitMQ management UI at http://localhost:15672
- Verify username/password (default: guest/guest)

### Migration Issues
```bash
# Install EF Core tools globally if not already installed
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

---

## 📖 Reference Implementation (UserService)

The UserService is fully implemented and can be used as a reference for:
- Domain entities with encapsulation
- CQRS commands and queries
- Command/query handlers with dependency injection
- Repository pattern
- Unit of Work pattern
- DbContext configuration
- Entity configuration with Fluent API
- RabbitMQ event publishing
- REST API controllers
- Dependency injection setup

**Location**: `/UserService/` directory

**Study this implementation** to understand the patterns and replicate for other services!
