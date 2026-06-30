namespace ApplicationService.BankTransactions.DTOs
{
    public record CreateWithdrawalRequest(int SenderAccountID, decimal Amount) { }
}
