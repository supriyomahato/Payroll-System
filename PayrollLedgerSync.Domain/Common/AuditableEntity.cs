namespace PayrollLedgerSync.Domain.Common;

/// <summary>
/// Base class for auditable entities.
/// Business rule: every persisted business record must track who/when created and changed it.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTime CreatedOnUtc { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public DateTime? LastModifiedOnUtc { get; protected set; }
    public string? LastModifiedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedOnUtc { get; protected set; }
    public string? DeletedBy { get; protected set; }

    protected void SetCreatedAudit(string createdBy, DateTime createdOnUtc)
    {
        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException("CreatedBy is required.", nameof(createdBy));
        }

        CreatedBy = createdBy.Trim();
        CreatedOnUtc = createdOnUtc;
    }

    protected void Touch(string modifiedBy, DateTime modifiedOnUtc)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentException("ModifiedBy is required.", nameof(modifiedBy));
        }

        LastModifiedBy = modifiedBy.Trim();
        LastModifiedOnUtc = modifiedOnUtc;
    }

    protected void MarkDeleted(string deletedBy, DateTime deletedOnUtc)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
        {
            throw new ArgumentException("DeletedBy is required.", nameof(deletedBy));
        }

        IsDeleted = true;
        DeletedBy = deletedBy.Trim();
        DeletedOnUtc = deletedOnUtc;
    }
}
