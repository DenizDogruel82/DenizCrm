using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetCurrentByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<Subscription?> GetByExternalIdAsync(
        string externalSubscriptionId,
        CancellationToken cancellationToken = default);
}
