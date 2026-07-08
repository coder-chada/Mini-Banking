namespace ApplicationService.Accounts.DTOs
{
    public sealed record GetAccountByIDResponse(
        string DNI,
        string Correo,
        int AccountID,
        string NumeroCuenta,
        string TipoCuenta,
        string Moneda,
        decimal Balance
    )
    { }
}
