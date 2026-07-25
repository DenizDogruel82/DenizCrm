using SaasAiCrm.Domain.Entities;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface ILeadRepository : IRepository<Lead>
{
    Task<IReadOnlyList<Lead>> GetByStatusAsync(
        Guid tenantId,
        LeadStatus status,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Lead>> GetByOwnerAsync(
        Guid tenantId,
        Guid ownerUserId,
        CancellationToken cancellationToken = default);
}
