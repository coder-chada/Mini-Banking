using Domain.Enums;
using DomainLogic.Exceptions;

namespace DomainLogic.Entities
{
    public class Idempotency
    {
        public string Key { get; private set; } = string.Empty;
        public string RequestHash { get; private set; } = string.Empty;
        public IdempotencyStatus Status { get; private set; }
        public string ResponseBody { get; private set; } = string.Empty;
        public int StatusCode { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public Idempotency(string key, string requestHash)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.EntityInvalidData,
                                               message: "Key cannot be null or empty");

            if (String.IsNullOrWhiteSpace(requestHash))
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.EntityInvalidData,
                                               message: "request-hash cannot be null or empty");

            this.Key = key;
            this.RequestHash = requestHash;
            this.Status = IdempotencyStatus.InProgress;
            this.CreatedAt = DateTime.UtcNow;
            this.ExpiresAt = DateTime.UtcNow.AddHours(24);
        }

        public Idempotency(string key,
                           string requestHash,
                           IdempotencyStatus status,
                           string responseBody,
                           int statusCode,
                           DateTime createdAt,
                           DateTime completedAt,
                           DateTime expiredAt)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.EntityInvalidData,
                                               message: "Key cannot be null or empty");

            if (String.IsNullOrWhiteSpace(requestHash))
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.EntityInvalidData,
                                               message: "request-hash cannot be null or empty");

            this.Key = key;
            this.RequestHash = requestHash;
            this.Status = status;
            this.ResponseBody = responseBody;
            this.StatusCode = statusCode;
            this.CreatedAt = createdAt;
            this.CompletedAt = completedAt;
            this.ExpiresAt = expiredAt;
        }

        public void MarkAsCompleted()
        {
            if (Status == IdempotencyStatus.Completed)
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.IdempotencyInvalidData,
                                               message: "It is already completed");

            if (String.IsNullOrWhiteSpace(ResponseBody))
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.IdempotencyInvalidData,
                                               message: "Response body cannot be null or empty");

            if (StatusCode == 0)
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.IdempotencyInvalidData,
                                               message: "Status code cannot be zero");

            this.Status = IdempotencyStatus.Completed;
            this.CompletedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed() =>
            this.Status = IdempotencyStatus.Failed;

        public void SetResponseBody(string responseBody)
        {
            if (String.IsNullOrWhiteSpace(responseBody))
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.IdempotencyInvalidData,
                                               message: "Response body cannot be null or empty");

            this.ResponseBody = responseBody;
        }

        public void SetStatusCode(int statusCode)
        {
            if (statusCode < 1)
                throw new DomainLogicException(errorCode: DomainLogicErrorCode.IdempotencyInvalidData,
                                               message: "Status code cannot be negative");

            this.StatusCode = statusCode;
        }
    }
}
