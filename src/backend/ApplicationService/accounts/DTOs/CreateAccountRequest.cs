using Domain.Enums;

namespace ApplicationService.Accounts.DTOs
{
    public sealed record CreateAccountRequest(
        string Numero,
        AccountType Tipo,
        CurrencyType Currency,
        int OwnerID
    )
    { }
}
