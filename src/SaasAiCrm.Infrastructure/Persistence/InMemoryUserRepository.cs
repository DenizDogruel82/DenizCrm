using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Infrastructure.Persistence;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;

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

    public Task<User?> GetByIdAsync(
        Guid tenantId,
        Guid id,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.SingleOrDefault(x => x.TenantId == tenantId && x.Id == id));

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.SingleOrDefault(x =>
            string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<User>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<User>>(
            _users.Where(x => x.TenantId == tenantId).ToArray());

    public Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }

    public void Update(User user) { }
    public void Remove(User user) => _users.Remove(user);
}
