using DomainLogic.Contracts;

namespace DomainLogic.Events
{
    public sealed record FundsDepositedEvent : IDomainEvent
    {
        public int TransactionID { get; init; }
        public int ReceiverID { get; init; }

        public DateTimeOffset OcurredOn => DateTimeOffset.Now;

        public FundsDepositedEvent(int transactionID, int receiverID)
        {
            this.TransactionID = transactionID;
            this.ReceiverID = receiverID;
        }
    }
}
