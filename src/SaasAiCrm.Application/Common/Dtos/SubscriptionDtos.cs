using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Common.Dtos;

public sealed record SubscriptionDto(
    Guid Id,
    Guid TenantId,
    string PlanCode,
    SubscriptionStatus Status,
    int SeatLimit,
    DateTime PeriodStartUtc,
    DateTime PeriodEndUtc,
    bool CancelAtPeriodEnd);

public sealed record CreateSubscriptionDto(
    string PlanCode,
    int SeatLimit,
    DateTime PeriodStartUtc,
    DateTime PeriodEndUtc);

public sealed record UpdateSubscriptionDto(
    string PlanCode,
    SubscriptionStatus Status,
    int SeatLimit,
    DateTime PeriodStartUtc,
    DateTime PeriodEndUtc,
    bool CancelAtPeriodEnd);
