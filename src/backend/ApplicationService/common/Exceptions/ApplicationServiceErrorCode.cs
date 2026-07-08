namespace ApplicationService.Common.Exceptions
{
    public static class ApplicationServiceErrorCode
    {
        public const string MissingOrInvalidData = "data.missing_or_null_values";
        public const string IdempotencyInvalid = "idempotency.idempotency_is_invalid";
        public const string IdempotencyConflict = "idempotency.idempotency_conflict";
        public const string DataNotFound = "data.doest_not_exist";
    }
}
