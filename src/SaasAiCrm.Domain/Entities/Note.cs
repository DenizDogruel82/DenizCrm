using SaasAiCrm.Domain.Common;

namespace SaasAiCrm.Domain.Entities;

public sealed class Note : TenantEntity
{
    public required string Content { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
}
