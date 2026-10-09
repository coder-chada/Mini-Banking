using ApplicationService.Common.Contracts;
using Domain.Enums;
using DomainLogic.Entities;
using Infrastructure.PersistenceModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class IdempotencyRepository : IIdempotencyRepository
    {
        private readonly MyDBContext _myDBContext;

        public IdempotencyRepository(MyDBContext myDBContext)
        {
            this._myDBContext = myDBContext;
        }

        public async Task<Idempotency> TryCreateAsync(
            string key,
            string requestHash,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var rowsInserted = await _myDBContext.Database.ExecuteSqlInterpolatedAsync($@"
                    INSERT INTO 
                        (idempotency_key
                        , request_hash
                        , status)
                    VALUES
                        ({key}
                        , {requestHash}
                        , {(int)IdempotencyStatus.InProgress});
                    ON CONFLICT (idempotency_key) DO NOTHING
                    ", cancellationToken).ConfigureAwait(false);

                var idempotency = await GetByAsync(
                    key: key, 
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (idempotency is null)
                {
                    throw new Exception("idempotency exists, but it can not get it");
                }

                if (rowsInserted == 0)
                {
                    idempotency.SetAsClaimed(); // row already existed
                }

                return idempotency;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private Idempotency TurnIntoIdempotency(IdempotencyEntity value)
        {
            var idempotency = new Idempotency(key: value.idempotency_key, requestHash: value.request_hash);
            return idempotency;
        }

        public async Task<Idempotency?> GetByAsync(
            string key,
            CancellationToken cancellationToken = default
        )
        {
            var idempotencyEntity = await _myDBContext
                .Idempotency.AsNoTracking()
                .FirstOrDefaultAsync(i => i.idempotency_key.Equals(key), cancellationToken)
                .ConfigureAwait(false);

            if (idempotencyEntity is null)
                return null;

            var idempotency = new Idempotency(
                idempotencyEntity.idempotency_key,
                idempotencyEntity.request_hash,
                (IdempotencyStatus)idempotencyEntity.status,
                idempotencyEntity.response_body,
                idempotencyEntity.status_code,
                idempotencyEntity.created_at,
                idempotencyEntity.completed_at,
                idempotencyEntity.expires_at
            );

            return idempotency;
        }

        public async Task MarkAsCompletedAsync(
            Idempotency idempotency,
            CancellationToken cancellationToken = default
        )
        {
            var idempotencyEntity = await _myDBContext
                .Idempotency.FindAsync(idempotency.Key, cancellationToken)
                .ConfigureAwait(false);

            if (idempotencyEntity is null)
                throw new Exception();

            idempotencyEntity.response_body = idempotency.ResponseBody;
            idempotencyEntity.status_code = idempotency.StatusCode;
            idempotencyEntity.status = (int)IdempotencyStatus.Completed;
            idempotencyEntity.completed_at = DateTime.UtcNow;
        }

        public async Task MarkAsFailedAsync(
            string key,
            CancellationToken cancellationToken = default
        )
        {
            var idempotencyEntity = await _myDBContext
                .Idempotency.FindAsync(key, cancellationToken)
                .ConfigureAwait(false);

            if (idempotencyEntity is null)
                throw new Exception();

            idempotencyEntity.status = (int)IdempotencyStatus.Failed;
        }
    }
}
