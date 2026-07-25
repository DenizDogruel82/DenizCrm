using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Leads;

public sealed class CreateLeadHandler(ILeadRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<CreateLeadCommand, LeadDto>
{
    public async Task<LeadDto> HandleAsync(CreateLeadCommand command,
        CancellationToken cancellationToken = default)
    {
        var d = command.Lead;
        var e = new Lead { TenantId = current.TenantId, CreatedByUserId = current.UserId,
            FirstName = d.FirstName, LastName = d.LastName, CompanyName = d.CompanyName,
            Email = d.Email, Phone = d.Phone, Source = d.Source, OwnerUserId = d.OwnerUserId };
        await repository.AddAsync(e, cancellationToken);
        await unit.SaveChangesAsync(cancellationToken); return e.ToDto();
    }
}

public sealed class UpdateLeadHandler(ILeadRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<UpdateLeadCommand, LeadDto?>
{
    public async Task<LeadDto?> HandleAsync(UpdateLeadCommand command,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (e?.TenantId != current.TenantId) return null;
        var d = command.Lead; e.FirstName = d.FirstName; e.LastName = d.LastName;
        e.CompanyName = d.CompanyName; e.Email = d.Email; e.Phone = d.Phone; e.Source = d.Source;
        e.Status = d.Status; e.Score = d.Score; e.OwnerUserId = d.OwnerUserId;
        e.UpdatedAtUtc = DateTime.UtcNow; repository.Update(e);
        await unit.SaveChangesAsync(cancellationToken); return e.ToDto();
    }
}

public sealed class DeleteLeadHandler(ILeadRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<DeleteLeadCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteLeadCommand command,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (e?.TenantId != current.TenantId) return false;
        repository.Remove(e); await unit.SaveChangesAsync(cancellationToken); return true;
    }
}

public sealed class ConvertLeadHandler(ILeadRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<ConvertLeadCommand, LeadDto?>
{
    public async Task<LeadDto?> HandleAsync(ConvertLeadCommand command,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (e?.TenantId != current.TenantId || e.Status == LeadStatus.Converted) return null;
        e.Status = LeadStatus.Converted; e.ConvertedCustomerId = command.CustomerId;
        e.ConvertedContactId = command.ContactId; e.ConvertedAtUtc = DateTime.UtcNow;
        e.UpdatedAtUtc = DateTime.UtcNow; repository.Update(e);
        await unit.SaveChangesAsync(cancellationToken); return e.ToDto();
    }
}

public sealed class GetLeadByIdHandler(ILeadRepository repository, ICurrentUser current)
    : IQueryHandler<GetLeadByIdQuery, LeadDto?>
{
    public async Task<LeadDto?> HandleAsync(GetLeadByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(query.Id, cancellationToken);
        return e?.TenantId == current.TenantId ? e.ToDto() : null;
    }
}

public sealed class GetLeadsHandler(ILeadRepository repository, ICurrentUser current)
    : IQueryHandler<GetLeadsQuery, PagedResultDto<LeadDto>>
{
    public async Task<PagedResultDto<LeadDto>> HandleAsync(GetLeadsQuery query,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Lead> values = query.Status.HasValue
            ? await repository.GetByStatusAsync(current.TenantId, query.Status.Value, cancellationToken)
            : query.OwnerUserId.HasValue
                ? await repository.GetByOwnerAsync(current.TenantId, query.OwnerUserId.Value, cancellationToken)
                : (await repository.GetAllAsync(cancellationToken))
                    .Where(x => x.TenantId == current.TenantId).ToArray();
        var items = values.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => x.ToDto()).ToArray();
        return new(items, query.PageNumber, query.PageSize, values.Count);
    }
}
