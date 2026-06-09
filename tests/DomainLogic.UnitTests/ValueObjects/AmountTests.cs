using DomainLogic.Exceptions;
using DomainLogic.ValueObjects;

namespace DomainLogic.UnitTests.ValueObjects
{
    public class AmountTests
    {
        [Fact]
        public void Constructor_WhenAmountValueIsNegative_ShouldThrowDomainLogicException()
        {
            // arrange
            var amountValue = -1m;

            // act, assert
            var domainException = Assert.Throws<DomainLogicException>(() => new Amount(amountValue));

            Assert.Equal("amount.negative_value_or_zero_value", domainException.ErrorCode);
        }

        [Fact]
        public void Constructor_WhenValueIsPositive_ShouldCreateAmount()
        {
            // Arrange
            var amountValue = 100m;

            // Act
            var amount = new Amount(amountValue);

            // Assert
            Assert.Equal(amountValue, amount.Value);
        }

        [Fact]
        public void Constructor_WhenValueIsZero_ShouldThrowDomainLogicException()
        {
            // Arrange
            var amountValue = 0m;

            // Act
            var domainException = Assert.Throws<DomainLogicException>(() => new Amount(amountValue));

            // Assert
            Assert.Equal("amount.negative_value_or_zero_value", domainException.ErrorCode);
        }
    }
}
