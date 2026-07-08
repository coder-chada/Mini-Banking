using ApplicationService.Accounts.DTOs;
using DomainLogic.ValueObjects;

namespace ApplicationService.Accounts.Contracts
{
    public interface IAccountService
    {
        Task<CreateAccountResponse> CreateAccountAsync(
            CreateAccountRequest accountDTO,
            CancellationToken cancellationToken = default
        );

        Task<GetAccountByIDResponse> GetAccountByAsync(
            int accountID,
            CancellationToken cancellationToken = default
        );

        Task<List<GetAccountsByUserIDResponse>> GetAccountsByAsync(
            UserID userID,
            CancellationToken cancellationToken = default
        );
    }
}
