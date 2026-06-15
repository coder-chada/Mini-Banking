using ApplicationService.Accounts.DTOs;

namespace ApplicationService.Accounts.Contracts
{
    public interface IAccountService
    {
        Task<CreateAccountDTOResponse> CreateAccountAsync(CreateAccountDTO accountDTO,
                                                          CancellationToken cancellation = default);

        Task<GetAccountByDTOResponse> GetAccountByAsync(int ID,
                                                        CancellationToken cancellationToken = default);
    }
}
