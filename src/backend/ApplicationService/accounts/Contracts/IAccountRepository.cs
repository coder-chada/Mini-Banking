using DomainLogic.Entities;

namespace ApplicationService.Accounts.Contracts
{
    public interface IAccountRepository
    {
        Task<int> AddAccountAsync(Account account,
                                  CancellationToken cancellationToken = default);

        Task<Account?> GetByAsync(int ID,
                                  CancellationToken cancellationToken = default);

        Task UpdateBalanceAsync(Account account,
                                CancellationToken cancellationToken = default);
    }
}
