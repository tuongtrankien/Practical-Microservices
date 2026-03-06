using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries;

// Query to get user by ID
public record GetUserByIdQuery(Guid UserId) 
    : IRequest<UserDto?>;

// Query to get user by email
public record GetUserByEmailQuery(string Email) 
    : IRequest<UserDto?>;

// Query to get all users
public record GetAllUsersQuery() 
    : IRequest<IEnumerable<UserDto>>;
