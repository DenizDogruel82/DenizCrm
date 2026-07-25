namespace SaasAiCrm.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? CreatedByUserId { get; init; }
}

public abstract class TenantEntity : AuditableEntity
{
    public required Guid TenantId { get; init; }
}
