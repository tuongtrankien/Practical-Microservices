using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands;

// Command to create a new user
public record CreateUserCommand(string Email, string FirstName, string LastName, string Password) 
    : IRequest<UserDto>;

// Command to update user profile
public record UpdateUserCommand(Guid UserId, string FirstName, string LastName) 
    : IRequest<UserDto>;

// Command to deactivate user
public record DeactivateUserCommand(Guid UserId) 
    : IRequest<bool>;

// Command to activate user
public record ActivateUserCommand(Guid UserId) 
    : IRequest<bool>;
