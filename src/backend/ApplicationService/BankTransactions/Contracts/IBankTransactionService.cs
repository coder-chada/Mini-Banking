using ApplicationService.BankTransactions.DTOs;

namespace ApplicationService.BankTransactions.Contracts
{
    public interface IBankTransactionService
    {
        Task<CreateDepositResponse> MakeDepositAsync(
            string idempotencyKey,
            string requestHash,
            CreateDepositRequest depositDTO,
            CancellationToken cancellationToken = default
        );

        Task<CreateWithdrawalResponse> MakeWithdrawalAsync(
            string idempotencyKey,
            string requestHash,
            CreateWithdrawalRequest withdrawalDTO,
            CancellationToken cancellationToken = default
        );
    }
}
