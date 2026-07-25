using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Tenants;

public sealed class TenantCommandHandlers(ITenantRepository repository, IUnitOfWork unit)
    : ICommandHandler<CreateTenantCommand, TenantDto>,
      ICommandHandler<UpdateTenantCommand, TenantDto?>
{
    public async Task<TenantDto> HandleAsync(CreateTenantCommand c, CancellationToken ct = default)
    {
        var d = c.Tenant; var e = new Tenant { Name = d.Name, Slug = d.Slug.ToLowerInvariant(),
            LogoUrl = d.LogoUrl, TimeZone = d.TimeZone, Currency = d.Currency.ToUpperInvariant() };
        await repository.AddAsync(e, ct); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<TenantDto?> HandleAsync(UpdateTenantCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e is null) return null;
        e.Name = c.Tenant.Name; e.LogoUrl = c.Tenant.LogoUrl; e.TimeZone = c.Tenant.TimeZone;
        e.Currency = c.Tenant.Currency.ToUpperInvariant(); e.IsActive = c.Tenant.IsActive;
        e.UpdatedAtUtc = DateTime.UtcNow; repository.Update(e); await unit.SaveChangesAsync(ct);
        return e.ToDto();
    }
}

public sealed class TenantQueryHandlers(ITenantRepository repository)
    : IQueryHandler<GetTenantByIdQuery, TenantDto?>,
      IQueryHandler<GetTenantBySlugQuery, TenantDto?>
{
    public async Task<TenantDto?> HandleAsync(GetTenantByIdQuery q, CancellationToken ct = default) =>
        (await repository.GetByIdAsync(q.Id, ct))?.ToDto();
    public async Task<TenantDto?> HandleAsync(GetTenantBySlugQuery q, CancellationToken ct = default) =>
        (await repository.GetBySlugAsync(q.Slug, ct))?.ToDto();
}
