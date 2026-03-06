namespace UserService.Application.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    bool IsActive
);

public record CreateUserDto(
    string Email,
    string FirstName,
    string LastName,
    string Password
);

public record UpdateUserDto(
    string FirstName,
    string LastName
);
