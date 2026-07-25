using SaasAiCrm.Domain.Entities;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface IAiInsightRepository : IRepository<AiInsight>
{
    Task<IReadOnlyList<AiInsight>> GetActiveAsync(
        Guid tenantId,
        AiInsightType? type = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AiInsight>> GetByLeadAsync(
        Guid tenantId,
        Guid leadId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AiInsight>> GetByOpportunityAsync(
        Guid tenantId,
        Guid opportunityId,
        CancellationToken cancellationToken = default);
}
