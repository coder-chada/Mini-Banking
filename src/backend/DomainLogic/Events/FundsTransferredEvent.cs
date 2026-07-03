using DomainLogic.Contracts;

namespace DomainLogic.Events
{
    public sealed record FundsTransferredEvent : IDomainEvent
    {
        public int TransactionID { get; init; }
        public int ReceiverID { get; init; }
        public int SenderID { get; init; }

        public DateTimeOffset OccurredOn => DateTimeOffset.Now;

        public FundsTransferredEvent(int transactionID, int senderID, int receiverID)
        {
            this.TransactionID = transactionID;
            this.SenderID = senderID;
            this.ReceiverID = receiverID;
        }

    }
}
