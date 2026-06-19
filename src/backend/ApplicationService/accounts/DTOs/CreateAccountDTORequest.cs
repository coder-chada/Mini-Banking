using Domain.Enums;

namespace ApplicationService.Accounts.DTOs
{
    public record CreateAccountDTORequest (string Numero, AccountType Tipo, CurrencyType Currency, int OwnerID)
    {
    }
}
