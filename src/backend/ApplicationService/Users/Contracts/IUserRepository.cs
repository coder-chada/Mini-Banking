using DomainLogic.Entities;

namespace ApplicationService.Users.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetBy(
            int userID,
            CancellationToken cancellationToken = default
        );

        Task<Func<int>> AddAsync(
            User user,
            CancellationToken cancellationToken = default
        );
    }
}
