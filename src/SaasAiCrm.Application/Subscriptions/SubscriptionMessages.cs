using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Subscriptions;

public sealed record CreateSubscriptionCommand(CreateSubscriptionDto Subscription)
    : ICommand<SubscriptionDto>;

public sealed record UpdateSubscriptionCommand(Guid Id, UpdateSubscriptionDto Subscription)
    : ICommand<SubscriptionDto?>;

public sealed record CancelSubscriptionCommand(Guid Id, bool AtPeriodEnd)
    : ICommand<SubscriptionDto?>;

public sealed record GetCurrentSubscriptionQuery : IQuery<SubscriptionDto?>;
