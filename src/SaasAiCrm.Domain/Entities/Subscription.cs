using SaasAiCrm.Domain.Common;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Domain.Entities;

public sealed class Subscription : AuditableEntity
{
    public required Guid TenantId { get; init; }
    public required string PlanCode { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trial;
    public int SeatLimit { get; set; } = 5;
    public DateTime PeriodStartUtc { get; set; }
    public DateTime PeriodEndUtc { get; set; }
    public string? ExternalCustomerId { get; set; }
    public string? ExternalSubscriptionId { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
}
