# ECommerce Microservices Architecture - Copilot Instructions

## Project Overview
This is an eCommerce system built using **Microservices Architecture** with **Domain-Driven Design (DDD)** and **Clean Architecture** patterns, implemented in **.NET 8**.

## Architecture Pattern

### Clean Architecture Layers
Each microservice follows Clean Architecture with the following layers:

1. **Domain Layer** (`*.Domain`)
   - Contains core business entities, value objects, domain events, and domain logic
   - Has **NO dependencies** on other layers
   - Represents the core business rules and domain model
   - Should be framework-agnostic

2. **Application Layer** (`*.Application`)
   - Contains application business logic, use cases, interfaces, DTOs, and commands/queries
   - **Depends on**: Domain layer only
   - Implements CQRS pattern (Commands and Queries)
   - Defines interfaces for infrastructure concerns (repositories, services)

3. **Infrastructure Layer** (`*.Infrastructure`)
   - Contains implementation of external concerns (database, external APIs, file system, etc.)
   - **Depends on**: Domain and Application layers
   - Implements repository interfaces from Application layer
   - Contains DbContext, Entity Framework configurations, external service clients
   - Handles all persistence and external integrations

4. **API Layer** (`*.API`)
   - ASP.NET Core Web API project
   - Contains controllers, middleware, filters, and configuration
   - **Depends on**: Application and Infrastructure layers
   - Entry point of the microservice
   - Handles HTTP requests and responses
   - Configures dependency injection

5. **Tests Layer** (`*.Tests`)
   - xUnit test project
   - **Depends on**: All layers (Domain, Application, Infrastructure, API)
   - Contains unit tests, integration tests, and functional tests

### Dependency Flow (Clean Architecture)
```
API → Application → Domain
  ↘     ↓
    Infrastructure → Application → Domain
    
Tests → API, Application, Infrastructure, Domain
```

### ⚠️ Exception: OrchestratorService

**The OrchestratorService does NOT follow Clean Architecture.**

It is a **lightweight, stateless background service** for saga orchestration:
- **No Database**: No DbContext, no repositories, no persistence
- **No Clean Architecture**: Only API (minimal hosting) + Infrastructure (consumers)
- **No Domain/Application layers**: Events are defined in Infrastructure
- **Purpose**: Event-driven workflow coordination (listen → orchestrate → publish)

This is because orchestrators should be simple event routers, not full-fledged microservices.


## Microservices

The system consists of 5 independent microservices, each with its own solution:

1. **NotificationService**
   - Handles email, SMS, and push notifications
   - Location: `/NotificationService/`
   - Solution: `NotificationService.sln`

2. **OrderService**
   - Manages order processing, order lifecycle, and order history
   - Location: `/OrderService/`
   - Solution: `OrderService.sln`

3. **PaymentService**
   - Handles payment processing, refunds, and payment methods
   - Location: `/PaymentService/`
   - Solution: `PaymentService.sln`

4. **ProductService**
   - Manages product catalog, inventory, and product information
   - Location: `/ProductService/`
   - Solution: `ProductService.sln`

5. **UserService**
   - Manages user accounts, authentication, authorization, and profiles
   - Location: `/UserService/`
   - Solution: `UserService.sln`

6. **OrchestratorService** ⚠️ (Exception - No Clean Architecture)
   - Saga orchestration for distributed transactions
   - Lightweight background service (stateless)
   - Location: `/OrchestratorService/`
   - Solution: `OrchestratorService.sln`
   - See [OrchestratorService/README.md](../OrchestratorService/README.md) for details


## Project Structure

Each microservice follows this structure:
```
<ServiceName>/
├── <ServiceName>.sln
├── <ServiceName>.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Events/
│   ├── Exceptions/
│   └── Interfaces/
├── <ServiceName>.Application/
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Mappings/
│   └── Services/
├── <ServiceName>.Infrastructure/
│   ├── Data/
│   │   ├── Configurations/
│   │   └── Repositories/
│   ├── Services/
│   └── ExternalServices/
├── <ServiceName>.API/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Filters/
│   └── Program.cs
└── <ServiceName>.Tests/
    ├── Unit/
    ├── Integration/
    └── Functional/
```

## Design Principles

### SOLID Principles
- **S**ingle Responsibility Principle: Each class has one reason to change
- **O**pen/Closed Principle: Open for extension, closed for modification
- **L**iskov Substitution Principle: Subtypes must be substitutable for base types
- **I**nterface Segregation Principle: Many specific interfaces over one general interface
- **D**ependency Inversion Principle: Depend on abstractions, not concretions

### Domain-Driven Design (DDD)
- **Entities**: Objects with unique identity
- **Value Objects**: Immutable objects without identity
- **Aggregates**: Cluster of entities and value objects with defined boundaries
- **Repositories**: Abstraction for data access
- **Domain Events**: Events that domain experts care about
- **Domain Services**: Operations that don't naturally fit in entities

### Microservices Principles
- Each microservice is independently deployable
- Each has its own database (Database per Service pattern)
- Services communicate via APIs or message brokers
- Each service owns its own domain logic

## Technology Stack
- **.NET 8**: Framework version
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM (to be configured)
- **xUnit**: Testing framework
- **MediatR**: CQRS pattern implementation (recommended)
- **FluentValidation**: Validation (recommended)
- **AutoMapper**: Object mapping (recommended)

## Coding Guidelines

### Naming Conventions
- Use PascalCase for classes, methods, properties
- Use camelCase for local variables and parameters
- Prefix interfaces with `I` (e.g., `IRepository`)
- Use meaningful, descriptive names

### File Organization
- One class per file
- File name should match class name
- Group related files in folders

### Dependency Injection
- Register services in respective layer configurations
- Use constructor injection
- Prefer interfaces over concrete implementations

### Error Handling
- Use domain exceptions for business rule violations
- Implement global exception handling in API layer
- Return appropriate HTTP status codes
- Log errors appropriately

### Testing
- Write unit tests for business logic
- Write integration tests for infrastructure
- Write functional tests for API endpoints
- Aim for high code coverage

## Common Patterns to Use

1. **Repository Pattern**: For data access abstraction
2. **Unit of Work**: For transaction management
3. **CQRS**: Separate read and write operations
4. **Mediator Pattern**: For decoupling request handling
5. **Factory Pattern**: For complex object creation
6. **Strategy Pattern**: For interchangeable algorithms
7. **Specification Pattern**: For business rules

## When Adding New Features

1. **Start with Domain**: Define entities, value objects, and domain logic
2. **Add Application Logic**: Create commands/queries, DTOs, and interfaces
3. **Implement Infrastructure**: Add repositories, external services, and data access
4. **Create API Endpoints**: Add controllers and configure routing
5. **Write Tests**: Cover all layers with appropriate tests
6. **Update Documentation**: Keep this file and XML documentation up to date

## Best Practices

- Keep domain logic in the Domain layer
- Keep application logic in the Application layer
- Keep infrastructure concerns in the Infrastructure layer
- Don't reference Infrastructure from Application (use interfaces)
- Use dependency injection for loose coupling
- Implement proper logging and monitoring
- Use async/await for I/O operations
- Follow RESTful API conventions
- Version your APIs
- Document APIs with Swagger/OpenAPI
- Implement proper authentication and authorization
- Use DTOs for API contracts, never expose domain entities directly
- Validate input at API and Application layers
- Apply database migrations properly

## References & Dependencies

### Project Reference Structure
```
Domain: No references

Application: 
  → Domain

Infrastructure:
  → Domain
  → Application

API:
  → Application
  → Infrastructure

Tests:
  → Domain
  → Application
  → Infrastructure
  → API
```

## Event-Driven Messaging Architecture

### Services Using RabbitMQ
The following services use RabbitMQ/MassTransit for event-driven communication:

1. **ProductService**: Publishes product-related events
2. **OrderService**: Publishes order events & consumes payment events
3. **PaymentService**: Publishes payment events & consumes order events
4. **NotificationService**: Consumes events from all services (pure consumer)

### Service NOT Using RabbitMQ
- **UserService**: Standalone service with HTTP API only (no event publishing)

### Event Flow Pattern
```
ProductService ──► Events ──► NotificationService
OrderService ⇄ Events ⇄ PaymentService
                  ↓
            NotificationService

UserService: HTTP API only (no events)
```

### Implementation Notes
- Use MassTransit with RabbitMQ for messaging
- Event consumers should be idempotent
- Use proper error handling and retry policies
- Events should be immutable records
- See MESSAGING_ARCHITECTURE.md for details

## Future Considerations

- **API Gateway**: For unified entry point
- **Service Discovery**: For dynamic service location
- **Message Broker**: ✅ IMPLEMENTED (RabbitMQ for ProductService, OrderService, PaymentService, NotificationService)
- **Event Sourcing**: For audit trail and event replay
- **CQRS with separate databases**: ✅ IMPLEMENTED (Each service has its own database)
- **Containerization**: Docker containers
- **Orchestration**: Kubernetes
- **Observability**: Logging, metrics, and distributed tracing
- **Resilience**: Circuit breakers, retries, timeouts
- **API Versioning**: For backward compatibility

## Notes for Copilot

When implementing features:
1. Always respect the layer boundaries and dependency rules
2. Place code in the appropriate layer based on its responsibility
3. Use interfaces to decouple layers
4. Follow the established patterns and conventions
5. Maintain the Clean Architecture structure
6. Each microservice is independent - avoid cross-service references
7. When in doubt, favor domain-centric design over infrastructure convenience
