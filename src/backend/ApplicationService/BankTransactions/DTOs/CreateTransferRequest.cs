namespace ApplicationService.BankTransactions.DTOs
{
    public sealed record CreateTransferRequest(int SenderAccountID, int ReceiverAccountID, decimal Amount)
    {
    }
}
