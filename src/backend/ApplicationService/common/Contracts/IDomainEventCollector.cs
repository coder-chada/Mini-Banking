using MediatR;

namespace ApplicationService.Common.Contracts
{
    public interface IDomainEventCollector
    {
        void AddEvents(IEnumerable<INotification> events);

        IReadOnlyCollection<INotification> GetAll();

        void Clear();
    }
}
