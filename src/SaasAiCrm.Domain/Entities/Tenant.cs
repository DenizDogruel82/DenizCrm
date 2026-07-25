using SaasAiCrm.Domain.Common;

namespace SaasAiCrm.Domain.Entities;

public sealed class Tenant : AuditableEntity
{
    public required string Name { get; set; }
    public required string Slug { get; init; }
    public string? LogoUrl { get; set; }
    public string TimeZone { get; set; } = "Europe/Istanbul";
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; } = true;
}
