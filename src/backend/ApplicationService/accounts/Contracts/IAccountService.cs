using ApplicationService.Accounts.DTOs;

namespace ApplicationService.Accounts.Contracts
{
    public interface IAccountService
    {
        Task<CreateAccountDTOResponse> CreateAccountAsync(CreateAccountDTORequest accountDTO,
                                                          CancellationToken cancellationToken = default);

        Task<GetAccountByDTOResponse> GetAccountByAsync(int ID,
                                                        CancellationToken cancellationToken = default);
    }
}
