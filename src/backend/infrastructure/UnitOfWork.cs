using ApplicationService.Accounts.Contracts;
using ApplicationService.BankTransactions.Contracts;
using ApplicationService.Common.Contracts;
using ApplicationService.Users.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly MyDBContext _myDBContext;
        private IUserRepository _userRepository;
        private IAccountRepository _accountRepository;

        public UnitOfWork(MyDBContext myDBContext,
                          IUserRepository userRepository,
                          IAccountRepository accountRepository)
        {
            this._myDBContext = myDBContext;
            this._userRepository = userRepository;
            this._accountRepository = accountRepository;
        }

        public IUserRepository UserRepository
        {
            get => _userRepository;
        }

        public IAccountRepository AccountRepository
        {
            get => _accountRepository;
        }

        public IBankTransactionRepository BankTransactionRepository => throw new NotImplementedException();

        public IIdempotencyRepository IdempotencyRepository => throw new NotImplementedException();

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _myDBContext
                .Database
                .BeginTransactionAsync()
                .ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _myDBContext
                .Database
                .CommitTransactionAsync()
                .ConfigureAwait(false);
        }

        public void Dispose() =>
            _myDBContext.Dispose();

        public async Task DisposeAsync()
        {
            await _myDBContext
                .DisposeAsync()
                .ConfigureAwait(false);
        }

        public Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _myDBContext
                .Database
                .RollbackTransactionAsync()
                .ConfigureAwait(false);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var rowsAffected = await _myDBContext
                    .SaveChangesAsync()
                    .ConfigureAwait(false);

                return rowsAffected;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException("The record was modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
