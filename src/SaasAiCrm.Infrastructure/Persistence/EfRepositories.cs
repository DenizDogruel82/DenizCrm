using Microsoft.EntityFrameworkCore;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Domain.Common;
using SaasAiCrm.Domain.Entities;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Infrastructure.Persistence;

internal abstract class EfRepository<TEntity>(CrmDbContext db) : IRepository<TEntity>
    where TEntity : Entity
{
    protected CrmDbContext Db { get; } = db;

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Db.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await Db.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await Db.Set<TEntity>().AddAsync(entity, cancellationToken);

    public void Update(TEntity entity) => Db.Set<TEntity>().Update(entity);
    public void Remove(TEntity entity) => Db.Set<TEntity>().Remove(entity);
}

internal sealed class TenantRepository(CrmDbContext db)
    : EfRepository<Tenant>(db), ITenantRepository
{
    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        Db.Tenants.FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default) =>
        Db.Tenants.AnyAsync(x => x.Slug == slug, cancellationToken);
}

internal sealed class CustomerRepository(CrmDbContext db)
    : EfRepository<Customer>(db), ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetByTenantAsync(
        Guid tenantId, CancellationToken cancellationToken = default) =>
        await Db.Customers.AsNoTracking().Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Customer>> SearchAsync(
        Guid tenantId, string searchTerm, CancellationToken cancellationToken = default) =>
        await Db.Customers.AsNoTracking()
            .Where(x => x.TenantId == tenantId &&
                        (x.Name.Contains(searchTerm) ||
                         (x.Email != null && x.Email.Contains(searchTerm))))
            .OrderBy(x => x.Name).ToListAsync(cancellationToken);
}

internal sealed class ContactRepository(CrmDbContext db)
    : EfRepository<Contact>(db), IContactRepository
{
    public async Task<IReadOnlyList<Contact>> GetByCustomerAsync(
        Guid tenantId, Guid customerId, CancellationToken cancellationToken = default) =>
        await Db.Contacts.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId)
            .OrderByDescending(x => x.IsPrimary).ThenBy(x => x.FirstName)
            .ToListAsync(cancellationToken);

    public Task<Contact?> GetByEmailAsync(
        Guid tenantId, string email, CancellationToken cancellationToken = default) =>
        Db.Contacts.FirstOrDefaultAsync(
            x => x.TenantId == tenantId && x.Email == email, cancellationToken);
}

internal sealed class LeadRepository(CrmDbContext db)
    : EfRepository<Lead>(db), ILeadRepository
{
    public async Task<IReadOnlyList<Lead>> GetByStatusAsync(
        Guid tenantId, LeadStatus status, CancellationToken cancellationToken = default) =>
        await Db.Leads.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Status == status)
            .OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Lead>> GetByOwnerAsync(
        Guid tenantId, Guid ownerUserId, CancellationToken cancellationToken = default) =>
        await Db.Leads.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
}

internal sealed class OpportunityRepository(CrmDbContext db)
    : EfRepository<Opportunity>(db), IOpportunityRepository
{
    public async Task<IReadOnlyList<Opportunity>> GetByStageAsync(
        Guid tenantId, Guid pipelineStageId, CancellationToken cancellationToken = default) =>
        await Filter(tenantId).Where(x => x.PipelineStageId == pipelineStageId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Opportunity>> GetByCustomerAsync(
        Guid tenantId, Guid customerId, CancellationToken cancellationToken = default) =>
        await Filter(tenantId).Where(x => x.CustomerId == customerId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Opportunity>> GetByStatusAsync(
        Guid tenantId, OpportunityStatus status, CancellationToken cancellationToken = default) =>
        await Filter(tenantId).Where(x => x.Status == status).ToListAsync(cancellationToken);

    private IQueryable<Opportunity> Filter(Guid tenantId) =>
        Db.Opportunities.AsNoTracking().Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAtUtc);
}

internal sealed class PipelineStageRepository(CrmDbContext db)
    : EfRepository<PipelineStage>(db), IPipelineStageRepository
{
    public async Task<IReadOnlyList<PipelineStage>> GetOrderedByTenantAsync(
        Guid tenantId, CancellationToken cancellationToken = default) =>
        await Db.PipelineStages.AsNoTracking().Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Order).ToListAsync(cancellationToken);
}

internal sealed class ActivityRepository(CrmDbContext db)
    : EfRepository<Activity>(db), IActivityRepository
{
    public async Task<IReadOnlyList<Activity>> GetUpcomingAsync(
        Guid tenantId, Guid? assignedUserId, DateTime untilUtc,
        CancellationToken cancellationToken = default) =>
        await Db.Activities.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Status == ActivityStatus.Planned &&
                        x.DueAtUtc <= untilUtc &&
                        (!assignedUserId.HasValue || x.AssignedUserId == assignedUserId))
            .OrderBy(x => x.DueAtUtc).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Activity>> GetByStatusAsync(
        Guid tenantId, ActivityStatus status, CancellationToken cancellationToken = default) =>
        await Db.Activities.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Status == status)
            .OrderBy(x => x.DueAtUtc).ToListAsync(cancellationToken);
}

internal sealed class NoteRepository(CrmDbContext db)
    : EfRepository<Note>(db), INoteRepository
{
    public async Task<IReadOnlyList<Note>> GetByCustomerAsync(
        Guid tenantId, Guid customerId, CancellationToken cancellationToken = default) =>
        await Db.Notes.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Note>> GetByOpportunityAsync(
        Guid tenantId, Guid opportunityId, CancellationToken cancellationToken = default) =>
        await Db.Notes.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.OpportunityId == opportunityId)
            .OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
}

internal sealed class AiInsightRepository(CrmDbContext db)
    : EfRepository<AiInsight>(db), IAiInsightRepository
{
    public async Task<IReadOnlyList<AiInsight>> GetActiveAsync(
        Guid tenantId, AiInsightType? type = null, CancellationToken cancellationToken = default) =>
        await Db.AiInsights.AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDismissed &&
                        (!x.ExpiresAtUtc.HasValue || x.ExpiresAtUtc > DateTime.UtcNow) &&
                        (!type.HasValue || x.Type == type))
            .OrderByDescending(x => x.GeneratedAtUtc).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AiInsight>> GetByLeadAsync(
        Guid tenantId, Guid leadId, CancellationToken cancellationToken = default) =>
        await Db.AiInsights.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.LeadId == leadId)
            .OrderByDescending(x => x.GeneratedAtUtc).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AiInsight>> GetByOpportunityAsync(
        Guid tenantId, Guid opportunityId, CancellationToken cancellationToken = default) =>
        await Db.AiInsights.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.OpportunityId == opportunityId)
            .OrderByDescending(x => x.GeneratedAtUtc).ToListAsync(cancellationToken);
}

internal sealed class SubscriptionRepository(CrmDbContext db)
    : EfRepository<Subscription>(db), ISubscriptionRepository
{
    public Task<Subscription?> GetCurrentByTenantAsync(
        Guid tenantId, CancellationToken cancellationToken = default) =>
        Db.Subscriptions.OrderByDescending(x => x.PeriodEndUtc)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

    public Task<Subscription?> GetByExternalIdAsync(
        string externalSubscriptionId, CancellationToken cancellationToken = default) =>
        Db.Subscriptions.FirstOrDefaultAsync(
            x => x.ExternalSubscriptionId == externalSubscriptionId, cancellationToken);
}
