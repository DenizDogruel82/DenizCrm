using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.AiInsights;

public sealed class AiInsightCommandHandlers(IAiInsightRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<CreateAiInsightCommand, AiInsightDto>,
    ICommandHandler<DismissAiInsightCommand, AiInsightDto?>,
    ICommandHandler<DeleteAiInsightCommand, bool>
{
    public async Task<AiInsightDto> HandleAsync(CreateAiInsightCommand c,
        CancellationToken ct = default)
    {
        var d = c.Insight; var e = new AiInsight { TenantId = current.TenantId,
            CreatedByUserId = current.UserId, Type = d.Type, Title = d.Title, Content = d.Content,
            Score = d.Score, Confidence = d.Confidence, CustomerId = d.CustomerId,
            LeadId = d.LeadId, OpportunityId = d.OpportunityId, Model = d.Model,
            ExpiresAtUtc = d.ExpiresAtUtc };
        await repository.AddAsync(e, ct); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<AiInsightDto?> HandleAsync(DismissAiInsightCommand c,
        CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return null;
        e.IsDismissed = c.IsDismissed; e.UpdatedAtUtc = DateTime.UtcNow; repository.Update(e);
        await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<bool> HandleAsync(DeleteAiInsightCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return false;
        repository.Remove(e); await unit.SaveChangesAsync(ct); return true;
    }
}

public sealed class AiInsightQueryHandlers(IAiInsightRepository repository, ICurrentUser current)
    : IQueryHandler<GetAiInsightByIdQuery, AiInsightDto?>,
      IQueryHandler<GetAiInsightsQuery, IReadOnlyList<AiInsightDto>>,
      IQueryHandler<GetLeadAiInsightsQuery, IReadOnlyList<AiInsightDto>>,
      IQueryHandler<GetOpportunityAiInsightsQuery, IReadOnlyList<AiInsightDto>>
{
    public async Task<AiInsightDto?> HandleAsync(GetAiInsightByIdQuery q, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(q.Id, ct);
        return e?.TenantId == current.TenantId ? e.ToDto() : null;
    }
    public async Task<IReadOnlyList<AiInsightDto>> HandleAsync(GetAiInsightsQuery q,
        CancellationToken ct = default) => Map(await repository.GetActiveAsync(current.TenantId, q.Type, ct));
    public async Task<IReadOnlyList<AiInsightDto>> HandleAsync(GetLeadAiInsightsQuery q,
        CancellationToken ct = default) => Map(await repository.GetByLeadAsync(current.TenantId, q.LeadId, ct));
    public async Task<IReadOnlyList<AiInsightDto>> HandleAsync(GetOpportunityAiInsightsQuery q,
        CancellationToken ct = default) => Map(await repository.GetByOpportunityAsync(
            current.TenantId, q.OpportunityId, ct));
    private static IReadOnlyList<AiInsightDto> Map(IReadOnlyList<AiInsight> values) =>
        values.Select(x => x.ToDto()).ToArray();
}
