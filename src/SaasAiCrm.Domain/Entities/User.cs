using SaasAiCrm.Domain.Common;

namespace SaasAiCrm.Domain.Entities;

public sealed class User : TenantEntity
{
    public required string Email { get; init; }
    public required string FullName { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = "Admin";
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAtUtc { get; set; }
}
