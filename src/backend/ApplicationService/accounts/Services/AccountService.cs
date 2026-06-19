using ApplicationService.Accounts.Contracts;
using ApplicationService.Common.Contracts;
using ApplicationService.Accounts.DTOs;
using DomainLogic.Entities;
using ApplicationService.Common.Exceptions;

namespace ApplicationService.Accounts.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<CreateAccountDTOResponse> CreateAccountAsync(CreateAccountDTORequest accountDTO,
                                                                       CancellationToken cancellationToken = default)
        {
            var account = new Account(numero: accountDTO.Numero,
                                      tipo: accountDTO.Tipo,
                                      currency: accountDTO.Currency,
                                      ownerID: accountDTO.OwnerID,
                                      balance: 0);
                
            var newAccountID = await _unitOfWork
                .AccountRepository
                .AddAccountAsync(account, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            var response = new CreateAccountDTOResponse(newAccountID());

            return response;
        }

        public async Task<GetAccountByDTOResponse> GetAccountByAsync(int id,
                                                                     CancellationToken cancellationToken = default)
        {
            var account = await _unitOfWork.AccountRepository
                .GetByAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (account is null)
                throw new ApplicationServiceException(ApplicationServiceErrorCode.MissingOrInvalidData,
                                                      "Account can not be null");

            var user = await _unitOfWork.UserRepository
                .GetUserBy(id, cancellationToken)
                .ConfigureAwait(false);

            if (user is null)
                throw new ApplicationServiceException(ApplicationServiceErrorCode.MissingOrInvalidData,
                                                      "User can not be null");

            var response = new GetAccountByDTOResponse(DNI: user.DNI,
                                                       Correo: user.Email,
                                                       AccountID: account.Id,
                                                       NumeroCuenta: account.Numero,
                                                       Balance: account.Balance);

            return response;
        }
    }
}
