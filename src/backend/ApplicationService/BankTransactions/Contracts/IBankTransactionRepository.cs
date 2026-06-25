using DomainLogic.Entities;

namespace ApplicationService.BankTransactions.Contracts
{
    public interface IBankTransactionRepository
    {
        Task<Guid> AddAsync(BankTransaction transaction,
                            CancellationToken cancellationToken = default);

        Task MarkAsCompletedAsync(Guid transactionID,
                                  CancellationToken cancellationToken = default);
    }
}
