using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface IPipelineStageRepository : IRepository<PipelineStage>
{
    Task<IReadOnlyList<PipelineStage>> GetOrderedByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
