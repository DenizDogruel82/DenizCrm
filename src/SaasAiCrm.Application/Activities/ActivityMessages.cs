using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Activities;

public sealed record CreateActivityCommand(CreateActivityDto Activity)
    : ICommand<ActivityDto>;

public sealed record UpdateActivityCommand(Guid Id, UpdateActivityDto Activity)
    : ICommand<ActivityDto?>;

public sealed record DeleteActivityCommand(Guid Id) : ICommand<bool>;

public sealed record CompleteActivityCommand(Guid Id, DateTime CompletedAtUtc)
    : ICommand<ActivityDto?>;

public sealed record GetActivityByIdQuery(Guid Id) : IQuery<ActivityDto?>;

public sealed record GetActivitiesQuery(
    ActivityStatus? Status = null,
    Guid? AssignedUserId = null,
    DateTime? UntilUtc = null) : IQuery<IReadOnlyList<ActivityDto>>;
