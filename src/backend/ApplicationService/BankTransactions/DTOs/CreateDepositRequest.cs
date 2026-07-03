namespace ApplicationService.BankTransactions.DTOs
{
    public sealed record CreateDepositRequest(int ReceiverAccountID, decimal Amount)
    {
    }
}
