namespace ApplicationService.Common.Contracts
{
    public interface IIdempotencyExecutor
    {
        Task<T> ExecuteAsync<T>(string key,
                                string requestHash,
                                Func<CancellationToken, Task<T>> businessLogicFunction,
                                CancellationToken cancellationToken = default);
    }
}
