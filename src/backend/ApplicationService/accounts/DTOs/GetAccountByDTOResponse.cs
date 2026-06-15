namespace ApplicationService.Accounts.DTOs
{
    public record GetAccountByDTOResponse(string DNI, string Correo, int AccountID, string NumeroCuenta, decimal Balance)
    {
    }
}
