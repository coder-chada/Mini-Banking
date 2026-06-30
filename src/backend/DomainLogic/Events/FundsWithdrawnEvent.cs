using DomainLogic.Contracts;

namespace DomainLogic.Events
{
    public sealed record FundsWithdrawnEvent : IDomainEvent
    {
        public int TransactionID { get; init; }
        public int SenderID { get; init; }
        public DateTimeOffset OccurredOn { get; init; }

        public FundsWithdrawnEvent(int transactionID, int senderID)
        {
            this.TransactionID = transactionID;
            this.SenderID = senderID;
            this.OccurredOn = DateTime.UtcNow;
        }
    }
}
