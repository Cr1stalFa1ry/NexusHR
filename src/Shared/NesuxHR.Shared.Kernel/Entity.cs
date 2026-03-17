using System;

namespace NexusHR.Shared.Kernel;

public abstract class Entity
{
    public Guid Id { get; private init; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdateAt { get; private set; }

    protected Entity()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkUpdated() => UpdateAt = DateTimeOffset.UtcNow;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
