using DomainLogic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationService.Common.Contracts
{
    public interface IIdempotencyRepository
    {
        Task CreateInProgressAsync(string key,
                                   string requestHash,
                                   CancellationToken cancellationToken = default);

        Task<Idempotency?> GetByAsync(string key,
                                      CancellationToken cancellationToken = default);

        Task MarkAsCompletedAsync(Idempotency idempotency,
                                  CancellationToken cancellationToken = default);

        Task MarkAsFailedAsync(string key,
                               CancellationToken cancellationToken = default);
    }
}
