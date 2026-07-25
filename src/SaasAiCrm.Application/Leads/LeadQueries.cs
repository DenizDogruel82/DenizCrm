using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Leads;

public sealed record GetLeadByIdQuery(Guid Id) : IQuery<LeadDto?>;

public sealed record GetLeadsQuery(
    LeadStatus? Status = null,
    Guid? OwnerUserId = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResultDto<LeadDto>>;
