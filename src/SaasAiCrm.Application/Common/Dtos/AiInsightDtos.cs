using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Common.Dtos;

public sealed record AiInsightDto(
    Guid Id,
    AiInsightType Type,
    string Title,
    string Content,
    decimal? Score,
    decimal? Confidence,
    Guid? CustomerId,
    Guid? LeadId,
    Guid? OpportunityId,
    string Model,
    DateTime GeneratedAtUtc,
    DateTime? ExpiresAtUtc,
    bool IsDismissed);

public sealed record CreateAiInsightDto(
    AiInsightType Type,
    string Title,
    string Content,
    decimal? Score,
    decimal? Confidence,
    Guid? CustomerId,
    Guid? LeadId,
    Guid? OpportunityId,
    string Model,
    DateTime? ExpiresAtUtc);

public sealed record DismissAiInsightDto(bool IsDismissed);
