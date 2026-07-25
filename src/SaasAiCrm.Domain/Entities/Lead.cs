using SaasAiCrm.Domain.Common;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Domain.Entities;

public sealed class Lead : TenantEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Source { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public int Score { get; set; }
    public Guid? OwnerUserId { get; set; }
    public Guid? ConvertedCustomerId { get; set; }
    public Guid? ConvertedContactId { get; set; }
    public DateTime? ConvertedAtUtc { get; set; }
}
