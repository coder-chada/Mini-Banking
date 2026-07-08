namespace ApplicationService.Accounts.DTOs
{
    public sealed record GetAccountsByUserIDResponse(
        string DNI,
        string Correo,
        int AccountID,
        string NumeroCuenta,
        string TipoCuenta,
        string Moneda,
        decimal Balance
    )
    {
    }
}
