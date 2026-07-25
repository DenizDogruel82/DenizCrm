using SaasAiCrm.Domain.Common;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Domain.Entities;

public sealed class Activity : TenantEntity
{
    public required string Subject { get; set; }
    public string? Description { get; set; }
    public ActivityType Type { get; set; }
    public ActivityStatus Status { get; set; } = ActivityStatus.Planned;
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public DateTime? DueAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
