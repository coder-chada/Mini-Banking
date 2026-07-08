using DomainLogic.Exceptions;

namespace DomainLogic.ValueObjects
{
    public sealed class Amount
    {
        const decimal MinValue = (decimal)0.01;
        public decimal Value { get; private set; }

        public Amount(decimal value)
        {
            if (value < MinValue || value == 0)
                throw new DomainLogicException(DomainLogicErrorCode.InvalidAmount, "amount value can not be zero or negative");

            this.Value = value;
        }

        public override bool Equals(object? obj) =>
            obj is Amount other && this.Value == other.Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public static bool operator ==(Amount left, Amount right) =>
            left.Equals(right);

        public static bool operator !=(Amount left, Amount right) =>
            !left.Equals(right);
    }
}
