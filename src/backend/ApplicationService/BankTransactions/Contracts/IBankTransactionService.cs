using ApplicationService.BankTransactions.DTOs;

namespace ApplicationService.BankTransactions.Contracts
{
    public interface IBankTransactionService
    {
        Task<CreateDepositResponse> MakeDepositAsync(string idempotencyKey,
                                                     string requestHash,
                                                     CreateDepositRequest depositDTO,
                                                     CancellationToken cancellationToken = default);
    }
}
