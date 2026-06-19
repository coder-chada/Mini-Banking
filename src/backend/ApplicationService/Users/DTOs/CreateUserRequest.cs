namespace ApplicationService.Users.DTOs;

public record CreateUserRequest(
    string DNI,
    string Nombres,
    string Apellidos,
    string Email
);
