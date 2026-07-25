using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.AiInsights;

public sealed record CreateAiInsightCommand(CreateAiInsightDto Insight)
    : ICommand<AiInsightDto>;

public sealed record DismissAiInsightCommand(Guid Id, bool IsDismissed)
    : ICommand<AiInsightDto?>;

public sealed record DeleteAiInsightCommand(Guid Id) : ICommand<bool>;

public sealed record GetAiInsightByIdQuery(Guid Id) : IQuery<AiInsightDto?>;

public sealed record GetAiInsightsQuery(AiInsightType? Type = null)
    : IQuery<IReadOnlyList<AiInsightDto>>;

public sealed record GetLeadAiInsightsQuery(Guid LeadId)
    : IQuery<IReadOnlyList<AiInsightDto>>;

public sealed record GetOpportunityAiInsightsQuery(Guid OpportunityId)
    : IQuery<IReadOnlyList<AiInsightDto>>;
