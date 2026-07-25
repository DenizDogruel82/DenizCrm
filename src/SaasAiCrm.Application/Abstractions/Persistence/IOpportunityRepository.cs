using SaasAiCrm.Domain.Entities;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface IOpportunityRepository : IRepository<Opportunity>
{
    Task<IReadOnlyList<Opportunity>> GetByStageAsync(
        Guid tenantId,
        Guid pipelineStageId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Opportunity>> GetByCustomerAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Opportunity>> GetByStatusAsync(
        Guid tenantId,
        OpportunityStatus status,
        CancellationToken cancellationToken = default);
}
