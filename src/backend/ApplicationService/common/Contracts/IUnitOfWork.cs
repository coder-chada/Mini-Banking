using ApplicationService.Accounts.Contracts;
using ApplicationService.BankTransactions.Contracts;
using ApplicationService.Users.Contracts;

namespace ApplicationService.Common.Contracts
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IAccountRepository AccountRepository { get; }
        IBankTransactionRepository BankTransactionRepository { get; }
        IIdempotencyRepository IdempotencyRepository { get; }

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task PublishDomainEventsAsync(CancellationToken cancellationToken = default);
    }
}
