using ApplicationService.BankTransactions.Contracts;
using Domain.Enums;
using DomainLogic.Entities;
using Infrastructure.PersistenceModels;

namespace Infrastructure.Repositories
{
    internal class BankTransactionRepository : IBankTransactionRepository
    {
        private readonly MyDBContext _myDBContext;

        public BankTransactionRepository(MyDBContext myDBContext)
        {
            this._myDBContext = myDBContext;
        }

        public async Task<Guid> AddAsync(BankTransaction transaction, CancellationToken cancellationToken = default)
        {
            var newID = Guid.NewGuid();

            var transactionEntity = new BankTransactionEntity()
            {
                ID = newID,
                SenderFK = transaction.SenderID,
                ReceiverFK = transaction.ReceiverID,
                Amount = transaction.Amount.Value,
                TransactionType = (int)transaction.Type,
                CreatedAt = DateTime.UtcNow,
                TransactionStatus = (int)transaction.Status
            };

            await _myDBContext.BankTransactions
                .AddAsync(transactionEntity, cancellationToken)
                .ConfigureAwait(false);

            return newID;
        }

        public async Task MarkAsCompletedAsync(Guid transactionID, CancellationToken cancellationToken = default)
        {
            var transactionEntity = await _myDBContext.BankTransactions
                .FindAsync(transactionID, cancellationToken)
                .ConfigureAwait(false);

            if (transactionEntity is null)
                throw new Exception();

            transactionEntity.TransactionStatus = (int)BankTransactionStatus.Success;
        }
    }
}
