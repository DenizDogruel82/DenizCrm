using SaasAiCrm.Domain.Common;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Domain.Entities;

public sealed class AiInsight : TenantEntity
{
    public AiInsightType Type { get; init; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public decimal? Score { get; set; }
    public decimal? Confidence { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public required string Model { get; init; }
    public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; set; }
    public bool IsDismissed { get; set; }
}
