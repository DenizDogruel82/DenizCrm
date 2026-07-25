using SaasAiCrm.Domain.Common;

namespace SaasAiCrm.Domain.Entities;

public sealed class Contact : TenantEntity
{
    public required Guid CustomerId { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? JobTitle { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsPrimary { get; set; }
    public bool HasEmailConsent { get; set; }
}
