using MediatR;

namespace DomainLogic.Contracts
{
    public interface IDomainEvent : INotification
    {
        DateTimeOffset OcurredOn { get; }
    }
}
