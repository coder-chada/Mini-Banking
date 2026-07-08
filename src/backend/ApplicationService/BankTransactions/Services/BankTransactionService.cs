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
        private readonly IDomainEventCollector _eventCollector;

        public BankTransactionService(
            IUnitOfWork unitOfWork,
            IIdempotencyExecutor idempotencyExecutor,
            IDomainEventCollector eventCollector
        )
        {
            this._unitOfWork = unitOfWork;
            this._idempotencyExecutor = idempotencyExecutor;
            this._eventCollector = eventCollector;
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

            _eventCollector.AddEvents(deposit.GetEvents());
            deposit.ClearEvents();

            var newTransactionID = await _unitOfWork
                .BankTransactionRepository.AddAsync(transaction: deposit, cancellationToken: token)
                .ConfigureAwait(false);

            await _unitOfWork
                .AccountRepository.UpdateBalanceAsync(
                    account: accountReceiver,
                    cancellationToken: token
                )
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

        public async Task<CreateWithdrawalResponse> MakeWithdrawalAsync(
            string idempotencyKey,
            string requestHash,
            CreateWithdrawalRequest withdrawalDTO,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _idempotencyExecutor
                .ExecuteAsync(
                    idempotencyKey,
                    requestHash,
                    businessLogicFunction => DoWithdrawalAsync(withdrawalDTO, cancellationToken),
                    cancellationToken
                )
                .ConfigureAwait(false);

            return result;
        }

        private async Task<CreateWithdrawalResponse> DoWithdrawalAsync(
            CreateWithdrawalRequest withdrawalDTO,
            CancellationToken cancellationToken = default
        )
        {
            var account = await GetAccountByAsync(withdrawalDTO.SenderAccountID, cancellationToken)
                .ConfigureAwait(false);

            var bankTransaction = BankTransaction.FromWithdrawal(
                account,
                new Amount(withdrawalDTO.Amount)
            );
            bankTransaction.Execute();

            _eventCollector.AddEvents(bankTransaction.GetEvents());
            bankTransaction.ClearEvents();

            var newTransactionID = await _unitOfWork
                .BankTransactionRepository.AddAsync(bankTransaction, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork
                .AccountRepository.UpdateBalanceAsync(account, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork
                .BankTransactionRepository.MarkAsCompletedAsync(newTransactionID)
                .ConfigureAwait(false);

            return new CreateWithdrawalResponse(newTransactionID);
        }

        private async Task<Account> GetAccountByAsync(
            int accountID,
            CancellationToken cancellationToken
        )
        {
            var account = await _unitOfWork
                .AccountRepository.GetByAsync(accountID: accountID, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (account is null)
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.MissingOrInvalidData,
                    "Account does not exist"
                );

            return account;
        }

        public async Task<CreateTransferResponse> MakeTransferAsync(string idempotencyKey, string requestHash, CreateTransferRequest transferDTO, CancellationToken cancellationToken = default)
        {
            var result = await _idempotencyExecutor
                .ExecuteAsync(
                    idempotencyKey,
                    requestHash,
                    businessLogicFunction => DoTransferAsync(transferDTO, cancellationToken),
                    cancellationToken
                )
                .ConfigureAwait(false);

            return result;
        }

        private async Task<CreateTransferResponse> DoTransferAsync(CreateTransferRequest transferDTO, CancellationToken cancellationToken)
        {
            var senderAccount = await GetAccountByAsync(transferDTO.SenderAccountID, cancellationToken)
                .ConfigureAwait(false);

            var receiverAccount = await GetAccountByAsync(transferDTO.ReceiverAccountID, cancellationToken)
                .ConfigureAwait(false);

            var bankTransaction = BankTransaction.FromTransfer(
                sender: senderAccount,
                receiver: receiverAccount,
                amount: new Amount(transferDTO.Amount)
            );

            bankTransaction.Execute();

            _eventCollector.AddEvents(bankTransaction.GetEvents());
            bankTransaction.ClearEvents();

            var newTransactionID = await _unitOfWork
                .BankTransactionRepository.AddAsync(bankTransaction, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork
                .AccountRepository.UpdateBalanceAsync(senderAccount, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork
                .AccountRepository.UpdateBalanceAsync(receiverAccount, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork
                .BankTransactionRepository.MarkAsCompletedAsync(newTransactionID)
                .ConfigureAwait(false);

            return new CreateTransferResponse(newTransactionID);
        }
    }
}
