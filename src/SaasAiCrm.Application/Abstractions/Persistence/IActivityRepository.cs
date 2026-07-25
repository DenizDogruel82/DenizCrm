using SaasAiCrm.Domain.Entities;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface IActivityRepository : IRepository<Activity>
{
    Task<IReadOnlyList<Activity>> GetUpcomingAsync(
        Guid tenantId,
        Guid? assignedUserId,
        DateTime untilUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Activity>> GetByStatusAsync(
        Guid tenantId,
        ActivityStatus status,
        CancellationToken cancellationToken = default);
}
