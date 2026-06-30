using Domain.Enums;
using DomainLogic.Entities;
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
            Assert.NotEmpty(bankTransaction.GetEvents());
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
            Assert.NotEmpty(bankTransaction.GetEvents());
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
