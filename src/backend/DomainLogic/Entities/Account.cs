using Domain.Enums;
using DomainLogic.Exceptions;
using DomainLogic.Seedwork;
using DomainLogic.ValueObjects;

namespace DomainLogic.Entities
{
    public class Account : Entity
    {
        public string Numero { get; private set; } = string.Empty;
        public AccountType Tipo { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Balance { get; private set; }
        public int OwnerID { get; private set; }

        private Account() : base(0)
        {

        }

        public Account(string numero,
                       AccountType tipo,
                       CurrencyType currency,
                       int ownerID,
                       decimal balance,
                       int id = 0) : base(id)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new DomainLogicException(DomainLogicErrorCode.EntityInvalidData, "Account number is required.");

            if (ownerID <= 0)
                throw new DomainLogicException(DomainLogicErrorCode.EntityInvalidData, "Invalid owner ID.");

            if (balance < 0)
                throw new DomainLogicException(DomainLogicErrorCode.EntityInvalidData, "Balance cannot be negative.");

            this.Numero = numero;
            this.Tipo = tipo;
            this.Currency = currency;
            this.OwnerID = ownerID;
            this.Balance = balance;
        }

        public void Credit(Amount amount)
        {
            this.Balance += amount.Value;
        }

        public void Debit(Amount amount)
        {
            if (!HasSufficientFunds(amount.Value))
                throw new DomainLogicException(DomainLogicErrorCode.InsufficientBalance, "insufficience balance to the operation");

            this.Balance -= amount.Value;
        }

        public bool HasSufficientFunds(decimal amount) =>
            this.Balance >= amount;
    }
}
