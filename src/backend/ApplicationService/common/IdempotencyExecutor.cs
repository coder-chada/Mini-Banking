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

            Idempotency? idempotency = await GetIdempotencyBy(
                    key: key,
                    cancellationToken: cancellationToken
                )
                .ConfigureAwait(false);

            if (idempotency is not null)
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

            bool idempotencyClaimed = false;

            try
            {
                idempotency = new Idempotency(key, requestHash);

                await _unitOfWork.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

                // Claim idempotency for this request
                idempotencyClaimed = await ClaimIdempotencyAsync(idempotency, cancellationToken)
                    .ConfigureAwait(false);

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
                await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);

                // Mark idempotency as failed only if it was claimed earlier
                await MarkIdempotencyFailedIfClaimedAsync(
                        key,
                        idempotencyClaimed,
                        cancellationToken
                    )
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

        private async Task<bool> ClaimIdempotencyAsync(
            Idempotency idempotency,
            CancellationToken cancellationToken
        )
        {
            await _unitOfWork
                .IdempotencyRepository.CreateInProgressAsync(
                    idempotency.Key,
                    idempotency.RequestHash,
                    cancellationToken
                )
                .ConfigureAwait(false);

            return true;
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
            bool claimed,
            CancellationToken cancellationToken
        )
        {
            if (!claimed)
                return;

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
