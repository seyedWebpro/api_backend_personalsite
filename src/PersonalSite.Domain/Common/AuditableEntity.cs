namespace PersonalSite.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAtUtc { get; protected set; }

    public void MarkUpdated(DateTime utcNow) => UpdatedAtUtc = utcNow;

    public void SoftDelete(DateTime utcNow)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
    }
}
