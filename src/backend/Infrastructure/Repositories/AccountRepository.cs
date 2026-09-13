using ApplicationService.Accounts.Contracts;
using Domain.Enums;
using DomainLogic.Entities;
using DomainLogic.ValueObjects;
using Infrastructure.PersistenceModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly MyDBContext _myDBContext;

        public AccountRepository(MyDBContext myDBContext)
        {
            this._myDBContext = myDBContext;
        }

        public async Task<Func<int>> AddAccountAsync(
            Account account,
            CancellationToken cancellationToken = default
        )
        {
            var accountEntity = new AccountEntity(
                ID: account.ID,
                Numero: account.Numero,
                Tipo: (int)account.Tipo,
                Currency: (int)account.Currency,
                OwnerID: account.OwnerID,
                Balance: account.Balance
            );

            await _myDBContext
                .Accounts.AddAsync(accountEntity, cancellationToken)
                .ConfigureAwait(false);

            return () => accountEntity.ID;
        }

        public async Task<Account?> GetByAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var accountEntity = await _myDBContext
                .Accounts.AsNoTracking()
                .FirstOrDefaultAsync(a => a.ID == id, cancellationToken)
                .ConfigureAwait(false);

            if (accountEntity is null)
                return null;

            var account = new Account(
                id: accountEntity.ID,
                numero: accountEntity.Numero,
                tipo: (AccountType)accountEntity.Tipo,
                currency: (CurrencyType)accountEntity.Currency,
                ownerID: accountEntity.OwnerID,
                balance: accountEntity.Balance
            );

            return account;
        }

        public async Task<List<Account>> GetByAsync(
            UserID userID,
            CancellationToken cancellationToken = default
        )
        {
            var accountsEntities = await _myDBContext
                .Accounts.AsNoTracking()
                .Where(a => a.OwnerID == userID.Value)
                .ToListAsync(cancellationToken);

            var accounts = accountsEntities
                .Select(a => new Account(
                    numero: a.Numero,
                    tipo: (AccountType)a.Tipo,
                    currency: (CurrencyType)a.Currency,
                    ownerID: a.OwnerID,
                    balance: a.Balance,
                    id: a.ID
                ))
                .ToList();

            return accounts;
        }

        public async Task UpdateBalanceAsync(
            Account account,
            CancellationToken cancellationToken = default
        )
        {
            var rowsAffected = await _myDBContext
                .Accounts.Where(a => a.ID == account.ID)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(a => a.Balance, account.Balance),
                    cancellationToken
                );

            if (rowsAffected == 0)
                throw new Exception("None balance was updated");
        }
    }
}
