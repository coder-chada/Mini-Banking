using ApplicationService.BankTransactions.Contracts;
using ApplicationService.BankTransactions.DTOs;
using ApplicationService.Common.Contracts;
using ApplicationService.Common.Exceptions;
using DomainLogic.Entities;
using DomainLogic.ValueObjects;

namespace ApplicationService.BankTransactions.Services
{
    public class BankTransactionService : IBankTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdempotencyExecutor _idempotencyExecutor;

        public BankTransactionService(IUnitOfWork unitOfWork, IIdempotencyExecutor idempotencyExecutor)
        {
            this._unitOfWork = unitOfWork;
            this._idempotencyExecutor = idempotencyExecutor;
        }

        public async Task<CreateDepositResponse> MakeDepositAsync(
            string idempotencyKey,
            string requestHash,
            CreateDepositRequest depositDTO,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _idempotencyExecutor
                .ExecuteAsync(
                    key: idempotencyKey,
                    requestHash: requestHash,
                    businessLogicFunction => DoDepositAsync(depositDTO, cancellationToken),
                    cancellationToken: cancellationToken
                )
                .ConfigureAwait(false);

            return result;
        }

        private async Task<CreateDepositResponse> DoDepositAsync(
            CreateDepositRequest depositDTO,
            CancellationToken token = default
        )
        {
            var accountReceiver = await GetAccountByAsync(
                    accountID: depositDTO.ReceiverAccountID,
                    cancellationToken: token
                )
                .ConfigureAwait(false);

            var amount = new Amount(depositDTO.Amount);

            var deposit = BankTransaction.FromDeposit(receiver: accountReceiver, amount: amount);

            deposit.Execute();

            var newTransactionID = await _unitOfWork
                .BankTransactionRepository.AddAsync(transaction: deposit, cancellationToken: token)
                .ConfigureAwait(false);

            await _unitOfWork
                .AccountRepository.UpdateBalanceAsync(account: accountReceiver, cancellationToken: token)
                .ConfigureAwait(false);

            await _unitOfWork
                .BankTransactionRepository.MarkAsCompletedAsync(
                    transactionID: newTransactionID,
                    cancellationToken: token
                )
                .ConfigureAwait(false);

            var response = new CreateDepositResponse(ID: newTransactionID);

            return response;
        }

        private async Task<Account> GetAccountByAsync(int accountID, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork
                .AccountRepository.GetByAsync(ID: accountID, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (account is null)
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.MissingOrInvalidData,
                    "Account does not exist"
                );

            return account;
        }
    }
}
