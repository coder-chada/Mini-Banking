using ApplicationService.Accounts.Contracts;
using ApplicationService.BankTransactions.Contracts;
using ApplicationService.Common.Contracts;
using ApplicationService.Users.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly MyDBContext _myDBContext;
        private IUserRepository _userRepository;
        private IAccountRepository _accountRepository;
        private IBankTransactionRepository _bankTransactionRepository;
        private IIdempotencyRepository _idempotencyRepository;
        private readonly IDomainEventCollector _eventCollector;
        private readonly IMediator _mediator;

        public UnitOfWork(MyDBContext myDBContext,
                          IUserRepository userRepository,
                          IAccountRepository accountRepository,
                          IBankTransactionRepository bankTransactionRepository,
                          IIdempotencyRepository idempotencyRepository,
                          IDomainEventCollector eventCollector,
                          IMediator mediator)
        {
            this._myDBContext = myDBContext;
            this._userRepository = userRepository;
            this._accountRepository = accountRepository;
            this._bankTransactionRepository = bankTransactionRepository;
            this._idempotencyRepository = idempotencyRepository;
            this._eventCollector = eventCollector;
            this._mediator = mediator;
        }

        public IUserRepository UserRepository
        {
            get => _userRepository;
        }

        public IAccountRepository AccountRepository
        {
            get => _accountRepository;
        }

        public IBankTransactionRepository BankTransactionRepository
        {
            get => _bankTransactionRepository;
        }

        public IIdempotencyRepository IdempotencyRepository
        {
            get => _idempotencyRepository;
        }

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

        public async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = _eventCollector.GetAll();

            foreach (var domainEvet in domainEvents)
            {
                await _mediator
                    .Publish(domainEvet, cancellationToken)
                    .ConfigureAwait(false);
            }

            _eventCollector.Clear();
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
