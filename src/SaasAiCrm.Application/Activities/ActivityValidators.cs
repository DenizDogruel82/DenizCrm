using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Activities;

public sealed class CreateActivityDtoValidator : AbstractValidator<CreateActivityDto>
{
    public CreateActivityDtoValidator()
    {
        RuleFor(x => x.Subject).RequiredText(200);
        RuleFor(x => x.Description).OptionalText(2000);
        RuleFor(x => x).Must(x => x.CustomerId.HasValue || x.ContactId.HasValue ||
                                  x.LeadId.HasValue || x.OpportunityId.HasValue)
            .WithMessage("Aktivite en az bir CRM kaydıyla ilişkilendirilmelidir.");
    }
}

public sealed class UpdateActivityDtoValidator : AbstractValidator<UpdateActivityDto>
{
    public UpdateActivityDtoValidator()
    {
        RuleFor(x => x.Subject).RequiredText(200);
        RuleFor(x => x.Description).OptionalText(2000);
        RuleFor(x => x.CompletedAtUtc)
            .NotNull()
            .When(x => x.Status == Domain.Enums.ActivityStatus.Completed);
    }
}

public sealed class CreateActivityCommandValidator : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityCommandValidator(IValidator<CreateActivityDto> validator) =>
        RuleFor(x => x.Activity).NotNull().SetValidator(validator);
}

public sealed class UpdateActivityCommandValidator : AbstractValidator<UpdateActivityCommand>
{
    public UpdateActivityCommandValidator(IValidator<UpdateActivityDto> validator)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Activity).NotNull().SetValidator(validator);
    }
}
