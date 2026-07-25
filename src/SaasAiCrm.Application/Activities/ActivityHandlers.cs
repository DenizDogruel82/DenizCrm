using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Activities;

public sealed class ActivityCommandHandlers(IActivityRepository repository, IUnitOfWork unit,
    ICurrentUser current)
    : ICommandHandler<CreateActivityCommand, ActivityDto>,
      ICommandHandler<UpdateActivityCommand, ActivityDto?>,
      ICommandHandler<CompleteActivityCommand, ActivityDto?>,
      ICommandHandler<DeleteActivityCommand, bool>
{
    public async Task<ActivityDto> HandleAsync(CreateActivityCommand c, CancellationToken ct = default)
    {
        var d = c.Activity; var e = new Activity { TenantId = current.TenantId,
            CreatedByUserId = current.UserId, Subject = d.Subject, Description = d.Description,
            Type = d.Type, CustomerId = d.CustomerId, ContactId = d.ContactId, LeadId = d.LeadId,
            OpportunityId = d.OpportunityId, AssignedUserId = d.AssignedUserId, DueAtUtc = d.DueAtUtc };
        await repository.AddAsync(e, ct); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<ActivityDto?> HandleAsync(UpdateActivityCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return null;
        var d = c.Activity; e.Subject = d.Subject; e.Description = d.Description; e.Type = d.Type;
        e.Status = d.Status; e.AssignedUserId = d.AssignedUserId; e.DueAtUtc = d.DueAtUtc;
        e.CompletedAtUtc = d.CompletedAtUtc; e.UpdatedAtUtc = DateTime.UtcNow;
        repository.Update(e); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<ActivityDto?> HandleAsync(CompleteActivityCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return null;
        e.Status = ActivityStatus.Completed; e.CompletedAtUtc = c.CompletedAtUtc;
        e.UpdatedAtUtc = DateTime.UtcNow; repository.Update(e); await unit.SaveChangesAsync(ct);
        return e.ToDto();
    }
    public async Task<bool> HandleAsync(DeleteActivityCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return false;
        repository.Remove(e); await unit.SaveChangesAsync(ct); return true;
    }
}

public sealed class ActivityQueryHandlers(IActivityRepository repository, ICurrentUser current)
    : IQueryHandler<GetActivityByIdQuery, ActivityDto?>,
      IQueryHandler<GetActivitiesQuery, IReadOnlyList<ActivityDto>>
{
    public async Task<ActivityDto?> HandleAsync(GetActivityByIdQuery q, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(q.Id, ct);
        return e?.TenantId == current.TenantId ? e.ToDto() : null;
    }
    public async Task<IReadOnlyList<ActivityDto>> HandleAsync(GetActivitiesQuery q,
        CancellationToken ct = default)
    {
        var values = q.UntilUtc.HasValue
            ? await repository.GetUpcomingAsync(current.TenantId, q.AssignedUserId, q.UntilUtc.Value, ct)
            : await repository.GetByStatusAsync(current.TenantId, q.Status ?? ActivityStatus.Planned, ct);
        return values.Select(x => x.ToDto()).ToArray();
    }
}
