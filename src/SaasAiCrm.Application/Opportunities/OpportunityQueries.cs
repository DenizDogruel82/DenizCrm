using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Opportunities;

public sealed record GetOpportunityByIdQuery(Guid Id) : IQuery<OpportunityDto?>;

public sealed record GetOpportunitiesQuery(
    Guid? PipelineStageId = null,
    OpportunityStatus? Status = null,
    Guid? CustomerId = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResultDto<OpportunityDto>>;
