using Microsoft.EntityFrameworkCore;
using ApplicationService.Accounts.Contracts;
using DomainLogic.Entities;
using Mini_Banking.Domain.Enums;

namespace Infrastructure.Repositories
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly MyDBContext _myDBContext;

        public AccountRepository(MyDBContext myDBContext)
        {
            this._myDBContext = myDBContext;
        }

        public Task<int> AddAccountAsync(Account account,
                                         CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Account?> GetByAsync(int id,
                                               CancellationToken cancellationToken = default)
        {
            var accountEntity = await _myDBContext.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ID == id, cancellationToken)
                .ConfigureAwait(false);

            if (accountEntity is null)
                return null;

            var account = new Account(id: accountEntity.ID,
                                      numero: accountEntity.Numero,
                                      tipo: (AccountType)accountEntity.Tipo,
                                      currency: (CurrencyType)accountEntity.Currency,
                                      ownerID: accountEntity.OwnerID,
                                      balance: accountEntity.Balance);

            return account;
        }

        public async Task UpdateBalanceAsync(Account account,
                                             CancellationToken cancellationToken = default)
        {
            var rowsAffected = await _myDBContext.Accounts
                .Where(a => a.ID == account.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, account.Balance), cancellationToken);

            if (rowsAffected == 0)
                throw new Exception("None balance was updated");
        }
    }
}
