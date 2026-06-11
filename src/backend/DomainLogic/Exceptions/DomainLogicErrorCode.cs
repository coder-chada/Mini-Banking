namespace DomainLogic.Exceptions
{
    public static class DomainLogicErrorCode
    {
        public const string AccountIsNull = "account.account_not_be_null";
        public const string EntityInvalidData = "entity.invalid_data";
        public const string IdempotencyInvalidData = "idempotency.invalid_data";
        public const string InvalidAmount = "amount.negative_value_or_zero_value";
        public const string InsufficientBalance = "balance.insufficient_balance";
    }
}
