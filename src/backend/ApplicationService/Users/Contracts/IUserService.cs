using ApplicationService.Users.DTOs;

namespace ApplicationService.Users.Contracts
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(CreateUserDTO userDTO,
                                  CancellationToken cancellationToken = default);
    }
}