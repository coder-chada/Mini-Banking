using Mini_Banking.Domain.Enums;

namespace ApplicationService.Accounts.DTOs
{
    public record CreateAccountDTO (string Numero, AccountType Tipo, CurrencyType Currency, int OwnerID)
    {
    }
}
