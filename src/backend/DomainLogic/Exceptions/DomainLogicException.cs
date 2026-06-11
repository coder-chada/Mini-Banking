namespace DomainLogic.Exceptions
{
    public class DomainLogicException : Exception
    {
        public string ErrorCode { get; init; }
        public string? Details { get; init; }

        public DomainLogicException(string errorCode,
                                    string message) : base(message)
        {
            this.ErrorCode = errorCode;
        }

        public DomainLogicException(string errorCode,
                                    string message,
                                    string details) : base(message)
        {
            this.ErrorCode = errorCode;
            this.Details = details;
        }
    }
}
