using ApplicationService.Users.DTOs;

namespace ApplicationService.Users.Contracts
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest userDTO,
                                                 CancellationToken cancellationToken = default);
    }
}