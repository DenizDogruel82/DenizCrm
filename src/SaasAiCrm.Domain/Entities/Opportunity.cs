using SaasAiCrm.Domain.Common;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Domain.Entities;

public sealed class Opportunity : TenantEntity
{
    public required string Title { get; set; }
    public required Guid CustomerId { get; init; }
    public Guid? ContactId { get; set; }
    public required Guid PipelineStageId { get; set; }
    public Guid? OwnerUserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public OpportunityStatus Status { get; set; } = OpportunityStatus.Open;
    public int Probability { get; set; }
    public DateOnly? ExpectedCloseDate { get; set; }
    public string? LostReason { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
}
