namespace UserService.Domain.Events;

public record UserRegisteredEvent(Guid UserId, string Email, string FirstName, string LastName, DateTime RegisteredAt);

public record UserUpdatedEvent(Guid UserId, string Email, string FirstName, string LastName, DateTime UpdatedAt);

public record UserDeactivatedEvent(Guid UserId, DateTime DeactivatedAt);
