using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Common.Dtos;

public sealed record ActivityDto(
    Guid Id,
    string Subject,
    string? Description,
    ActivityType Type,
    ActivityStatus Status,
    Guid? CustomerId,
    Guid? ContactId,
    Guid? LeadId,
    Guid? OpportunityId,
    Guid? AssignedUserId,
    DateTime? DueAtUtc,
    DateTime? CompletedAtUtc,
    DateTime CreatedAtUtc);

public sealed record CreateActivityDto(
    string Subject,
    string? Description,
    ActivityType Type,
    Guid? CustomerId,
    Guid? ContactId,
    Guid? LeadId,
    Guid? OpportunityId,
    Guid? AssignedUserId,
    DateTime? DueAtUtc);

public sealed record UpdateActivityDto(
    string Subject,
    string? Description,
    ActivityType Type,
    ActivityStatus Status,
    Guid? AssignedUserId,
    DateTime? DueAtUtc,
    DateTime? CompletedAtUtc);
