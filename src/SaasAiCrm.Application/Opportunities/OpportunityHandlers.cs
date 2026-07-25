using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Opportunities;

public sealed class CreateOpportunityHandler(IOpportunityRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<CreateOpportunityCommand, OpportunityDto>
{
    public async Task<OpportunityDto> HandleAsync(CreateOpportunityCommand command,
        CancellationToken cancellationToken = default)
    {
        var d = command.Opportunity;
        var e = new Opportunity { TenantId = current.TenantId, CreatedByUserId = current.UserId,
            Title = d.Title, CustomerId = d.CustomerId, ContactId = d.ContactId,
            PipelineStageId = d.PipelineStageId, OwnerUserId = d.OwnerUserId, Amount = d.Amount,
            Currency = d.Currency.ToUpperInvariant(), Probability = d.Probability,
            ExpectedCloseDate = d.ExpectedCloseDate };
        await repository.AddAsync(e, cancellationToken);
        await unit.SaveChangesAsync(cancellationToken); return e.ToDto();
    }
}

public sealed class UpdateOpportunityHandler(IOpportunityRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<UpdateOpportunityCommand, OpportunityDto?>
{
    public async Task<OpportunityDto?> HandleAsync(UpdateOpportunityCommand command,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (e?.TenantId != current.TenantId) return null;
        var d = command.Opportunity; e.Title = d.Title; e.ContactId = d.ContactId;
        e.PipelineStageId = d.PipelineStageId; e.OwnerUserId = d.OwnerUserId; e.Amount = d.Amount;
        e.Currency = d.Currency.ToUpperInvariant(); e.Status = d.Status;
        e.Probability = d.Probability; e.ExpectedCloseDate = d.ExpectedCloseDate;
        e.LostReason = d.LostReason; e.UpdatedAtUtc = DateTime.UtcNow;
        repository.Update(e); await unit.SaveChangesAsync(cancellationToken); return e.ToDto();
    }
}

public sealed class DeleteOpportunityHandler(IOpportunityRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<DeleteOpportunityCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteOpportunityCommand command,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (e?.TenantId != current.TenantId) return false;
        repository.Remove(e); await unit.SaveChangesAsync(cancellationToken); return true;
    }
}

public sealed class ChangeOpportunityStageHandler(IOpportunityRepository repository,
    IUnitOfWork unit, ICurrentUser current)
    : ICommandHandler<ChangeOpportunityStageCommand, OpportunityDto?>
{
    public async Task<OpportunityDto?> HandleAsync(ChangeOpportunityStageCommand command,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (e?.TenantId != current.TenantId) return null;
        e.PipelineStageId = command.PipelineStageId; e.UpdatedAtUtc = DateTime.UtcNow;
        repository.Update(e); await unit.SaveChangesAsync(cancellationToken); return e.ToDto();
    }
}

public sealed class GetOpportunityByIdHandler(IOpportunityRepository repository,
    ICurrentUser current) : IQueryHandler<GetOpportunityByIdQuery, OpportunityDto?>
{
    public async Task<OpportunityDto?> HandleAsync(GetOpportunityByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(query.Id, cancellationToken);
        return e?.TenantId == current.TenantId ? e.ToDto() : null;
    }
}

public sealed class GetOpportunitiesHandler(IOpportunityRepository repository,
    ICurrentUser current) : IQueryHandler<GetOpportunitiesQuery, PagedResultDto<OpportunityDto>>
{
    public async Task<PagedResultDto<OpportunityDto>> HandleAsync(GetOpportunitiesQuery query,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Opportunity> values = query.PipelineStageId.HasValue
            ? await repository.GetByStageAsync(current.TenantId, query.PipelineStageId.Value, cancellationToken)
            : query.Status.HasValue
                ? await repository.GetByStatusAsync(current.TenantId, query.Status.Value, cancellationToken)
                : query.CustomerId.HasValue
                    ? await repository.GetByCustomerAsync(current.TenantId, query.CustomerId.Value, cancellationToken)
                    : (await repository.GetAllAsync(cancellationToken))
                        .Where(x => x.TenantId == current.TenantId).ToArray();
        var items = values.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => x.ToDto()).ToArray();
        return new(items, query.PageNumber, query.PageSize, values.Count);
    }
}
