using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Tenants;

public sealed record CreateTenantCommand(CreateTenantDto Tenant)
    : ICommand<TenantDto>;

public sealed record UpdateTenantCommand(Guid Id, UpdateTenantDto Tenant)
    : ICommand<TenantDto?>;

public sealed record GetTenantByIdQuery(Guid Id) : IQuery<TenantDto?>;

public sealed record GetTenantBySlugQuery(string Slug) : IQuery<TenantDto?>;
