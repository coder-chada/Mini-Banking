using DomainLogic.Entities;
using DomainLogic.Exceptions;
using DomainLogic.ValueObjects;
using Mini_Banking.Domain.Enums;

namespace DomainLogic.Test.Entities
{
    public class AccountTest
    {
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

        [Fact]
        public void Constructor_WhenBalanceIsNegative_ShouldThrowDomainLogicException()
        {
            // arrange
            decimal initialBalance = -10m;

            // act
            var domainException = Assert.Throws<DomainLogicException>(() => CreateAccount(initialBalance));

            // assert
            Assert.Equal(DomainLogicErrorCode.EntityInvalidData, domainException.ErrorCode);
        }

        [Fact]
        public void Constructor_WhenBalanceIsPositive_ShouldCreateAccount()
        {
            // arrange
            decimal initialBalance = 10m;

            // act
            var account = CreateAccount(initialBalance);

            // assert
            Assert.Equal(initialBalance, account.Balance);
        }

        [Fact]
        public void Credit_WhenCreditMyBalance_ShoudlIncreaseBalance()
        {
            // arrange
            decimal initialBalance = 10m;
            Amount vaueToCredit = new(10m);

            var account = CreateAccount(initialBalance);

            // act
            account.Credit(vaueToCredit);

            // assert
            Assert.Equal(initialBalance + vaueToCredit.Value, account.Balance);
        }

        [Fact]
        public void Debit_WhenDebitMyBalance_ShouldDecreaseBalance()
        {
            // arrange
            decimal initialBalance = 100m;
            Amount vaueToDebit = new(10m);

            var account = CreateAccount(initialBalance);

            // act
            account.Debit(vaueToDebit);

            // assert
            Assert.Equal(initialBalance - vaueToDebit.Value, account.Balance);
        }

        [Fact]
        public void Debit_WhenDebitAndHasInsufficientFunds_ShouldThrowDomainLogicException()
        {
            // arrange
            decimal initialBalance = 10m;
            Amount vaueToDebit = new(100m);

            var account = CreateAccount(initialBalance);

            // act
            var domainException = Assert.Throws<DomainLogicException>(() => account.Debit(vaueToDebit));

            // assert
            Assert.Equal(DomainLogicErrorCode.InsufficientBalance, domainException.ErrorCode);
        }

        [Fact]
        public void Constructor_WhenAccountNumberIsEmpty_ShouldThrowDomainLogicException()
        {
            // Arrange
            string emptyAccountNumber = string.Empty;

            // Act
            Action act = () => new Account(1, emptyAccountNumber, AccountType.Ahorro, CurrencyType.USD, 1, 0);

            // Assert
            var domainException = Assert.Throws<DomainLogicException>(act);
            Assert.Equal(DomainLogicErrorCode.EntityInvalidData, domainException.ErrorCode);
        }

        [Fact]
        public void Constructor_WhenOwnerIdIsZero_ShouldThrowDomainLogicException()
        {
            // Arrange
            int invalidOwnerId = 0;

            // Act
            Action act = () => new Account(1, "ACC-001", AccountType.Corriente, CurrencyType.USD, invalidOwnerId, 0);

            // Assert
            var exception = Assert.Throws<DomainLogicException>(act);
            Assert.Equal(DomainLogicErrorCode.EntityInvalidData, exception.ErrorCode);
        }

        [Fact]
        public void Constructor_WhenOwnerIdIsNegative_ShouldThrowDomainLogicException()
        {
            // Arrange
            int invalidOwnerId = -1;

            // Act
            Action act = () => new Account(1, "ACC-001", AccountType.Ahorro, CurrencyType.USD, invalidOwnerId, 0);

            // Assert
            var exception = Assert.Throws<DomainLogicException>(act);
            Assert.Equal(DomainLogicErrorCode.EntityInvalidData, exception.ErrorCode);
        }

        [Fact]
        public void Constructor_WhenBalanceIsZero_ShouldCreateAccount()
        {
            // arrange
            var initalBalance = 0m;

            // act
            var account = CreateAccount(initalBalance);

            // assert
            Assert.Equal(initalBalance, account.Balance);
        }

        [Fact]
        public void Debit_WhenAmountEqualsBalance_ShouldSetBalanceToZero()
        {
            // arrange 
            var initalBalance = 100m;
            Amount valueToDebit = new(initalBalance);
            var account = CreateAccount(initalBalance);

            // act
            account.Debit(valueToDebit);

            // assert
            Assert.Equal(0, account.Balance);
        }
    }
}
