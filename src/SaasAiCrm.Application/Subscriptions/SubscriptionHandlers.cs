using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Subscriptions;

public sealed class SubscriptionCommandHandlers(ISubscriptionRepository repository,
    IUnitOfWork unit, ICurrentUser current)
    : ICommandHandler<CreateSubscriptionCommand, SubscriptionDto>,
      ICommandHandler<UpdateSubscriptionCommand, SubscriptionDto?>,
      ICommandHandler<CancelSubscriptionCommand, SubscriptionDto?>
{
    public async Task<SubscriptionDto> HandleAsync(CreateSubscriptionCommand c,
        CancellationToken ct = default)
    {
        var d = c.Subscription; var e = new Subscription { TenantId = current.TenantId,
            CreatedByUserId = current.UserId, PlanCode = d.PlanCode, SeatLimit = d.SeatLimit,
            PeriodStartUtc = d.PeriodStartUtc, PeriodEndUtc = d.PeriodEndUtc };
        await repository.AddAsync(e, ct); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<SubscriptionDto?> HandleAsync(UpdateSubscriptionCommand c,
        CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return null;
        var d = c.Subscription; e.PlanCode = d.PlanCode; e.Status = d.Status; e.SeatLimit = d.SeatLimit;
        e.PeriodStartUtc = d.PeriodStartUtc; e.PeriodEndUtc = d.PeriodEndUtc;
        e.CancelAtPeriodEnd = d.CancelAtPeriodEnd; e.UpdatedAtUtc = DateTime.UtcNow;
        repository.Update(e); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<SubscriptionDto?> HandleAsync(CancelSubscriptionCommand c,
        CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return null;
        e.CancelAtPeriodEnd = c.AtPeriodEnd; e.UpdatedAtUtc = DateTime.UtcNow;
        repository.Update(e); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
}

public sealed class CurrentSubscriptionHandler(ISubscriptionRepository repository,
    ICurrentUser current) : IQueryHandler<GetCurrentSubscriptionQuery, SubscriptionDto?>
{
    public async Task<SubscriptionDto?> HandleAsync(GetCurrentSubscriptionQuery q,
        CancellationToken ct = default) =>
        (await repository.GetCurrentByTenantAsync(current.TenantId, ct))?.ToDto();
}
