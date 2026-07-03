using Domain.Enums;
using DomainLogic.Entities;
using DomainLogic.Events;
using DomainLogic.ValueObjects;

namespace DomainLogic.UnitTests.Entities
{
    public class BankTransactionTest
    {
        [Fact]
        public void Execute_WhenDepositIsSuccessful_AFundsDepositedEventMustExist()
        {
            // arrange
            var account = CreateAccount();
            var amount = new Amount(50);
            var bankTransaction = BankTransaction.FromDeposit(account, amount);

            // act
            bankTransaction.Execute();

            // assert
            var events = bankTransaction.GetEvents();
            var domainEvent = Assert.Single(events);

            Assert.IsType<FundsDepositedEvent>(domainEvent);
        }

        [Fact]
        public void Execute_WhenDepositIsSuccessful_ShouldCreditAccount()
        {
            // arrange
            var balance = 100m;
            var account = CreateAccount(balance);
            var amount = new Amount(50);
            var bankTransaction = BankTransaction.FromDeposit(account, amount);
            var expectedBalance = balance + amount.Value;

            // act
            bankTransaction.Execute();

            // assert
            Assert.Equal(expectedBalance, account.Balance);
        }

        [Fact]
        public void Execute_WhenDepositIsSuccessful_StatusMustBeToSuccess()
        {
            // arrange
            var account = CreateAccount();
            var amount = new Amount(50);
            var bankTransaction = BankTransaction.FromDeposit(account, amount);
            var expectedStatus = BankTransactionStatus.Success;

            // act
            bankTransaction.Execute();

            // assert
            Assert.Equal(expectedStatus, bankTransaction.Status);
        }

        [Fact]
        public void Execute_WhenDepositFails_AFundsDepositedEventMustNotExist()
        {
            // arrange
            var amount = new Amount(50);
            var bankTransaction = BankTransaction.FromDeposit(null!, amount);

            // act
            Action act = () => bankTransaction.Execute();

            // assert
            Assert.Empty(bankTransaction.GetEvents());
        }

        [Fact]
        public void Execute_WhenWithdrawnIsSuccessful_ShouldDebitAccount()
        {
            // arrange
            var balance = 100m;
            var account = CreateAccount(balance);
            var amount = new Amount(50);
            var bankTransaction = BankTransaction.FromWithdrawal(account, amount);
            var expectedBalance = balance - amount.Value;

            // act
            bankTransaction.Execute();

            // assert
            Assert.Equal(expectedBalance, account.Balance);
        }

        [Fact]
        public void Execute_WhenWithdrawnIsSuccessful_AFundsWithdrawnEventMustExist()
        {
            // arrange
            var account = CreateAccount();
            var amount = new Amount(50);
            var bankTransaction = BankTransaction.FromWithdrawal(account, amount);

            // act
            bankTransaction.Execute();

            // assert
            var events = bankTransaction.GetEvents();
            var domainEvent = Assert.Single(events);

            Assert.IsType<FundsWithdrawnEvent>(domainEvent);
        }

        [Fact]
        public void Execute_WhenTransferIsSuccessful_ShouldDebitSenderAccountCreditReceiverAccount()
        {
            // arrange
            var sender = CreateAccount();
            var receiver = CreateAccount();
            var amount = new Amount(50);
            var bankTransaction = BankTransaction.FromTransfer(sender, receiver, amount);
            var expectedSenderBalance = sender.Balance - amount.Value;
            var expectedReceiverBalance = receiver.Balance + amount.Value;

            // act
            bankTransaction.Execute();

            // assert
            Assert.Equal(expectedSenderBalance, sender.Balance);
            Assert.Equal(expectedReceiverBalance, receiver.Balance);
        }

        [Fact]
        public void Execute_WhenTransferIsSuccessful_AFundsTransferredEventMustExist()
        {
            // arrange
            var sender = CreateAccount();
            var receiver = CreateAccount();
            var amount = new Amount(50);
            var bankTransaction = BankTransaction.FromTransfer(sender, receiver, amount);

            // act
            bankTransaction.Execute();

            // assert
            var events = bankTransaction.GetEvents();
            var domainEvent = Assert.Single(events);

            Assert.IsType<FundsTransferredEvent>(domainEvent);
        }

        private static Account CreateAccount(decimal balance = 100m)
        {
            return new Account(
                id: 1,
                numero: "000",
                tipo: AccountType.Ahorro,
                currency: CurrencyType.USD,
                ownerID: 1,
                balance: balance);
        }
    }
}
