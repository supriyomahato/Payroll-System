namespace PayrollLedgerSync.Domain.Common;

/// <summary>
/// Base entity with identity semantics.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity other && other.GetType() == GetType() && other.Id == Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}
