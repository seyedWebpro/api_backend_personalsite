namespace PersonalSite.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}
