using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Subscriptions;

public sealed class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
{
    public CreateSubscriptionDtoValidator()
    {
        RuleFor(x => x.PlanCode).RequiredText(50);
        RuleFor(x => x.SeatLimit).GreaterThan(0);
        RuleFor(x => x.PeriodEndUtc).GreaterThan(x => x.PeriodStartUtc);
    }
}

public sealed class UpdateSubscriptionDtoValidator : AbstractValidator<UpdateSubscriptionDto>
{
    public UpdateSubscriptionDtoValidator()
    {
        RuleFor(x => x.PlanCode).RequiredText(50);
        RuleFor(x => x.SeatLimit).GreaterThan(0);
        RuleFor(x => x.PeriodEndUtc).GreaterThan(x => x.PeriodStartUtc);
    }
}
