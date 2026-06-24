using DomainLogic.Contracts;

namespace DomainLogic.Seedwork
{
    public class Entity
    {
        public int ID { get; set; }
        private readonly List<IDomainEvent> _domainEvents = new();


        protected Entity()
        {
            
        }

        protected Entity(int id)
        {
            this.ID = id;
        }

        public void RaiseEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearEvents() =>
            _domainEvents.Clear();

        public IReadOnlyCollection<IDomainEvent> GetEvents() =>
            _domainEvents.AsReadOnly();
    }
}
