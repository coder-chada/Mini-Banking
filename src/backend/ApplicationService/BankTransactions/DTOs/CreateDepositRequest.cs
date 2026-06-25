namespace ApplicationService.BankTransactions.DTOs
{
    public record CreateDepositRequest(int ReceiverAccountID, decimal Amount)
    {
    }
}
