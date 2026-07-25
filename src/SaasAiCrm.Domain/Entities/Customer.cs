using SaasAiCrm.Domain.Common;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Domain.Entities;

public sealed class Customer : TenantEntity
{
    public required string Name { get; set; }
    public CustomerType Type { get; set; } = CustomerType.Company;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public Guid? OwnerUserId { get; set; }
    public bool IsActive { get; set; } = true;
}
