using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.PipelineStages;

public sealed class PipelineStageCommandHandlers(IPipelineStageRepository repository,
    IUnitOfWork unit, ICurrentUser current)
    : ICommandHandler<CreatePipelineStageCommand, PipelineStageDto>,
      ICommandHandler<UpdatePipelineStageCommand, PipelineStageDto?>,
      ICommandHandler<DeletePipelineStageCommand, bool>
{
    public async Task<PipelineStageDto> HandleAsync(CreatePipelineStageCommand c,
        CancellationToken ct = default)
    {
        var d = c.Stage; var e = new PipelineStage { TenantId = current.TenantId,
            CreatedByUserId = current.UserId, Name = d.Name, Order = d.Order,
            WinProbability = d.WinProbability, Color = d.Color };
        await repository.AddAsync(e, ct); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<PipelineStageDto?> HandleAsync(UpdatePipelineStageCommand c,
        CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return null;
        e.Name = c.Stage.Name; e.Order = c.Stage.Order; e.WinProbability = c.Stage.WinProbability;
        e.Color = c.Stage.Color; e.IsActive = c.Stage.IsActive; e.UpdatedAtUtc = DateTime.UtcNow;
        repository.Update(e); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<bool> HandleAsync(DeletePipelineStageCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return false;
        repository.Remove(e); await unit.SaveChangesAsync(ct); return true;
    }
}

public sealed class PipelineStageQueryHandlers(IPipelineStageRepository repository,
    ICurrentUser current)
    : IQueryHandler<GetPipelineStageByIdQuery, PipelineStageDto?>,
      IQueryHandler<GetPipelineStagesQuery, IReadOnlyList<PipelineStageDto>>
{
    public async Task<PipelineStageDto?> HandleAsync(GetPipelineStageByIdQuery q,
        CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(q.Id, ct);
        return e?.TenantId == current.TenantId ? e.ToDto() : null;
    }
    public async Task<IReadOnlyList<PipelineStageDto>> HandleAsync(GetPipelineStagesQuery q,
        CancellationToken ct = default) =>
        (await repository.GetOrderedByTenantAsync(current.TenantId, ct)).Select(x => x.ToDto()).ToArray();
}
