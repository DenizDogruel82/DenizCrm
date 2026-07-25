namespace SaasAiCrm.Domain.Entities;

public sealed class User
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required string PasswordHash { get; set; }
    public string Role { get; init; } = "Admin";
    public bool IsActive { get; init; } = true;
}
