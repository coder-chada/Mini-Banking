namespace ApplicationService.Common.Exceptions
{
    public class ApplicationServiceException : Exception
    {
        public string Code { get; private set; } = string.Empty;
        public string? Details { get; private set; } = string.Empty;

        public ApplicationServiceException(string code, string message) : base(message)
        {
            this.Code = code;
        }
    }
}
