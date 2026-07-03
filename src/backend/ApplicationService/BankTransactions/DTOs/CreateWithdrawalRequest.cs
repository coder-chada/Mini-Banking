namespace ApplicationService.BankTransactions.DTOs
{
    public sealed record CreateWithdrawalRequest(int SenderAccountID, decimal Amount) { }
}
