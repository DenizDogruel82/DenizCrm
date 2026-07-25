using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Infrastructure.Persistence;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly IReadOnlyList<User> _users;

    public InMemoryUserRepository(IPasswordService passwords)
    {
        var admin = new User
        {
            TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "admin@saasaicrm.com",
            FullName = "CRM Yöneticisi",
            PasswordHash = string.Empty
        };
        admin.PasswordHash = passwords.Hash(admin, "Admin123!");
        _users = [admin];
    }

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.SingleOrDefault(x =>
            string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase)));
}
