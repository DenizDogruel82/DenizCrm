using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Opportunities;

public sealed record CreateOpportunityCommand(CreateOpportunityDto Opportunity)
    : ICommand<OpportunityDto>;

public sealed record UpdateOpportunityCommand(Guid Id, UpdateOpportunityDto Opportunity)
    : ICommand<OpportunityDto?>;

public sealed record DeleteOpportunityCommand(Guid Id) : ICommand<bool>;

public sealed record ChangeOpportunityStageCommand(Guid Id, Guid PipelineStageId)
    : ICommand<OpportunityDto?>;
