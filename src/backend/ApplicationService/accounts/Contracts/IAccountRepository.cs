using DomainLogic.Entities;
using DomainLogic.ValueObjects;

namespace ApplicationService.Accounts.Contracts
{
    public interface IAccountRepository
    {
        Task<Func<int>> AddAccountAsync(
            Account account,
            CancellationToken cancellationToken = default
        );

        Task<Account?> GetByAsync(
            int accountID,
            CancellationToken cancellationToken = default
        );

        Task<List<Account>> GetByAsync(
            UserID userID,
            CancellationToken cancellationToken = default
        );

        Task UpdateBalanceAsync(
            Account account,
            CancellationToken cancellationToken = default
        );
    }
}
