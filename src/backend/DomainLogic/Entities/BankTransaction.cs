using Domain.Enums;
using DomainLogic.Events;
using DomainLogic.Exceptions;
using DomainLogic.Seedwork;
using DomainLogic.ValueObjects;

namespace DomainLogic.Entities
{
    public class BankTransaction : Entity
    {
        public int? SenderID { get; private set; }
        public Account? SenderAccount { get; private set; }
        public int? ReceiverID { get; private set; }
        public Account? ReceiverAccount { get; private set; }
        public Amount Amount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public TransactionType Type { get; private set; }
        public BankTransactionStatus Status { get; private set; }

        private BankTransaction(Account? senderAccount,
                                Account? receiverAccount,
                                Amount amount,
                                TransactionType type,
                                BankTransactionStatus status)
        {
            this.SenderID = senderAccount is null ? null : senderAccount.ID;
            this.SenderAccount = senderAccount;
            this.ReceiverID = receiverAccount is null ? null : receiverAccount.ID;
            this.ReceiverAccount = receiverAccount;
            this.Amount = amount;
            this.CreatedAt = DateTime.UtcNow;
            this.Type = type;
            this.Status = status;
        }

        public static BankTransaction FromDeposit(Account receiver,
                                                  Amount amount) =>
            new(null,
                receiver,
                amount,
                TransactionType.Deposit,
                BankTransactionStatus.Pending);

        public static BankTransaction FromWithdrawal(Account sender, Amount amount) =>
            new(sender,
                 null,
                 amount,
                 TransactionType.Withdrawal,
                 BankTransactionStatus.Pending);

        private void MarkSuccessTransaction() =>
            this.Status = BankTransactionStatus.Success;

        public void Execute()
        {
            try
            {
                switch (this.Type)
                {
                    case TransactionType.Deposit:
                        ExecuteDeposit();
                        break;
                    case TransactionType.Withdrawal:
                        ExecuteWithdrawal();
                        break;
                    default:
                        break;
                }

                MarkSuccessTransaction();
            }
            catch (DomainLogicException)
            {
                MarkFailedTransaction();
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ExecuteDeposit()
        {
            if (ReceiverAccount is null)
                throw new DomainLogicException(DomainLogicErrorCode.AccountIsNull, "the receiver can not be null");

            ReceiverAccount.Credit(Amount);

            var fundDepositEvent = new FundsDepositedEvent(this.ID, this.ReceiverID!.Value);
            RaiseEvent(fundDepositEvent);
        }

        private void ExecuteWithdrawal()
        {
            if (SenderAccount is null)
                throw new DomainLogicException(DomainLogicErrorCode.AccountIsNull, "the sender can not be null");

            SenderAccount.Debit(Amount);

            var fundsWithdrawnEvent = new FundsWithdrawnEvent(0, SenderAccount.OwnerID);
            RaiseEvent(fundsWithdrawnEvent);
        }

        private void MarkFailedTransaction() =>
            this.Status = BankTransactionStatus.Failed;
    }
}
