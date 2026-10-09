using ApplicationService.Common.Contracts;
using ApplicationService.Common.Exceptions;
using Domain.Enums;
using DomainLogic.Entities;
using System.Text.Json;

namespace ApplicationService.Common
{
    public class IdempotencyExecutor : IIdempotencyExecutor
    {
        private readonly IUnitOfWork _unitOfWork;

        public IdempotencyExecutor(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<T> ExecuteAsync<T>(
            string key,
            string requestHash,
            Func<CancellationToken, Task<T>> businessLogicFunction,
            CancellationToken cancellationToken = default
        )
        {
            ValidateKeyRequestHash(key, requestHash);

            Idempotency idempotency = await TryClaimIdempotencyAsync(key, requestHash, cancellationToken)
                    .ConfigureAwait(false);

            if (idempotency.Status == IdempotencyStatus.Completed || idempotency.Status == IdempotencyStatus.Failed)
            {
                ValidateIdempotency(requestHash, idempotency);

                var result =
                    JsonSerializer.Deserialize<T>(idempotency.ResponseBody)
                    ?? throw new ApplicationServiceException(
                        ApplicationServiceErrorCode.IdempotencyInvalid,
                        $"Cached idempotency response could not be deserialized to {typeof(T).Name}."
                    );

                return result;
            }

            if (idempotency.Status == IdempotencyStatus.InProgress && idempotency.IsAlreadyClaimed)
            {
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.IdempotencyConflict,
                    $"the idempotency {key} was already claimed, try the action with a new idempotency key"
                );
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

                // Execute business logic and persist changes
                var response = await businessLogicFunction(cancellationToken).ConfigureAwait(false);

                // Persist idempotency success and commit
                var responseBody = JsonSerializer.Serialize<T>(response);
                await PersistIdempotencySuccessAsync(idempotency, responseBody, cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                await _unitOfWork.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);

                await _unitOfWork.PublishDomainEventsAsync(cancellationToken).ConfigureAwait(false);

                return response;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None).ConfigureAwait(false);

                await MarkIdempotencyFailedIfClaimedAsync(key, CancellationToken.None)
                    .ConfigureAwait(false);

                throw;
            }
        }

        private static void ValidateKeyRequestHash(string key, string requestHash)
        {
            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(requestHash))
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.IdempotencyInvalid,
                    "key or request hash can not be null or empty"
                );
        }

        private async Task<Idempotency?> GetIdempotencyBy(
            string key,
            CancellationToken cancellationToken
        )
        {
            return await _unitOfWork
                .IdempotencyRepository.GetByAsync(key, cancellationToken)
                .ConfigureAwait(false);
        }

        private static void ValidateIdempotency(string requestHash, Idempotency idempotency)
        {
            if (idempotency.RequestHash != requestHash)
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.IdempotencyConflict,
                    "Request mismatch"
                );

            if (idempotency.Status == IdempotencyStatus.InProgress)
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.IdempotencyConflict,
                    "Idempotency in progress"
                );

            if (idempotency.Status == IdempotencyStatus.Failed)
                throw new ApplicationServiceException(
                    ApplicationServiceErrorCode.IdempotencyConflict,
                    "Idempotency failed"
                );
        }

        private async Task<Idempotency> TryClaimIdempotencyAsync(
            string key,
            string requestHash,
            CancellationToken cancellationToken
        )
        {
            var idempotency = await _unitOfWork.IdempotencyRepository
                .TryCreateAsync(
                    key,
                    requestHash,
                    cancellationToken
                )
                .ConfigureAwait(false);

            return idempotency;
        }

        private async Task PersistIdempotencySuccessAsync(
            Idempotency idempotency,
            string responseBody,
            CancellationToken cancellationToken
        )
        {
            // 3. Build response (business result, NOT HTTP)
            idempotency.SetResponseBody(responseBody);
            idempotency.SetStatusCode(200);
            idempotency.MarkAsCompleted();

            // 4. Mark idempotency as completed
            await _unitOfWork
                .IdempotencyRepository.MarkAsCompletedAsync(idempotency, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task MarkIdempotencyFailedIfClaimedAsync(
            string key,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                await _unitOfWork
                    .IdempotencyRepository.MarkAsFailedAsync(key, cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Swallow to avoid masking the original exception; consider logging here.
            }
        }
    }
}
