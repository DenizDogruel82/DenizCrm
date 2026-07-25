using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Common.Dtos;

public sealed record OpportunityDto(
    Guid Id,
    string Title,
    Guid CustomerId,
    Guid? ContactId,
    Guid PipelineStageId,
    Guid? OwnerUserId,
    decimal Amount,
    string Currency,
    OpportunityStatus Status,
    int Probability,
    DateOnly? ExpectedCloseDate,
    string? LostReason,
    DateTime? ClosedAtUtc,
    DateTime CreatedAtUtc);

public sealed record CreateOpportunityDto(
    string Title,
    Guid CustomerId,
    Guid? ContactId,
    Guid PipelineStageId,
    Guid? OwnerUserId,
    decimal Amount,
    string Currency,
    int Probability,
    DateOnly? ExpectedCloseDate);

public sealed record UpdateOpportunityDto(
    string Title,
    Guid? ContactId,
    Guid PipelineStageId,
    Guid? OwnerUserId,
    decimal Amount,
    string Currency,
    OpportunityStatus Status,
    int Probability,
    DateOnly? ExpectedCloseDate,
    string? LostReason);
