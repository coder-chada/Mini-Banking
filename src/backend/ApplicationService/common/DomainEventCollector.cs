using ApplicationService.Common.Contracts;
using MediatR;

namespace ApplicationService.Common
{
    public class DomainEventCollector : IDomainEventCollector
    {

        private readonly List<INotification> _domainEvents = new();

        public void AddEvents(IEnumerable<INotification> events) =>
            _domainEvents.AddRange(events);

        public void Clear() =>
            _domainEvents.Clear();

        public IReadOnlyCollection<INotification> GetAll() =>
            _domainEvents.AsReadOnly();
    }
}
