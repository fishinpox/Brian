using MediatR;

namespace Shared.Infrastructure.Common;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}
