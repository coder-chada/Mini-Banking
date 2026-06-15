namespace ApplicationService.Users.DTOs;

public record CreateUserDTO(
    string DNI,
    string Nombres,
    string Apellidos,
    string Email
);
