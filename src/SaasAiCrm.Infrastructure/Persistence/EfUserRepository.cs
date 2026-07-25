using Microsoft.EntityFrameworkCore;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Infrastructure.Persistence;

internal sealed class EfUserRepository(CrmDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(
        Guid tenantId,
        Guid id,
        CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(
            x => x.TenantId == tenantId && x.Id == id,
            cancellationToken);

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(
            x => x.Email == email,
            cancellationToken);

    public async Task<IReadOnlyList<User>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await db.Users.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.FullName)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default) =>
        await db.Users.AddAsync(user, cancellationToken);

    public void Update(User user) => db.Users.Update(user);
    public void Remove(User user) => db.Users.Remove(user);
}
