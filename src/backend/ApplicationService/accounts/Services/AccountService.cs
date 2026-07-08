using ApplicationService.Accounts.Contracts;
using ApplicationService.Accounts.DTOs;
using ApplicationService.Common.Contracts;
using ApplicationService.Common.Exceptions;
using DomainLogic.Entities;
using DomainLogic.ValueObjects;

namespace ApplicationService.Accounts.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<CreateAccountResponse> CreateAccountAsync(
            CreateAccountRequest accountDTO,
            CancellationToken cancellationToken = default
        )
        {
            var account = new Account(
                numero: accountDTO.Numero,
                tipo: accountDTO.Tipo,
                currency: accountDTO.Currency,
                ownerID: accountDTO.OwnerID,
                balance: 0
            );

            var newAccountID = await _unitOfWork
                .AccountRepository.AddAccountAsync(account, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var response = new CreateAccountResponse(newAccountID());

            return response;
        }

        public async Task<GetAccountByIDResponse> GetAccountByAsync(
            int accountID,
            CancellationToken cancellationToken = default
        )
        {
            var account = await _unitOfWork
                .AccountRepository.GetByAsync(
                    accountID: accountID,
                    cancellationToken: cancellationToken
                )
                .ConfigureAwait(false);

            if (account is null)
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.MissingOrInvalidData,
                    "Account does not exists"
                );

            var user = await _unitOfWork
                .UserRepository.GetBy(userID: account.OwnerID, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (user is null)
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.MissingOrInvalidData,
                    "User can not be null"
                );

            var response = new GetAccountByIDResponse(
                DNI: user.DNI,
                Correo: user.Email,
                AccountID: account.ID,
                NumeroCuenta: account.Numero,
                TipoCuenta: account.Tipo.ToString(),
                Moneda: account.Currency.ToString(),
                Balance: account.Balance
            );

            return response;
        }

        public async Task<List<GetAccountsByUserIDResponse>> GetAccountsByAsync(
            UserID userID,
            CancellationToken cancellationToken = default
        )
        {
            var accounts = await _unitOfWork
                .AccountRepository.GetByAsync(userID: userID, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (!accounts.Any())
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.DataNotFound,
                    $"does not exist accounts for the User-ID {userID.Value}"
                );

            var ownerID = accounts.First().OwnerID;

            var ownerAccount = await _unitOfWork
                .UserRepository.GetBy(userID: ownerID, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (ownerAccount is null)
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.DataNotFound,
                    $"does not exist an owner for the User-ID {ownerID}"
                );

            var result = accounts
                .Select(account => new GetAccountsByUserIDResponse(
                    DNI: ownerAccount.DNI,
                    Correo: ownerAccount.Email,
                    AccountID: account.ID,
                    NumeroCuenta: account.Numero,
                    TipoCuenta: account.Tipo.ToString(),
                    Moneda: account.Currency.ToString(),
                    Balance: account.Balance
                ))
                .ToList();

            return result;
        }
    }
}
