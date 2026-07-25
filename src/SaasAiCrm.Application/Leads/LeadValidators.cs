using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Leads;

public sealed class CreateLeadDtoValidator : AbstractValidator<CreateLeadDto>
{
    public CreateLeadDtoValidator()
    {
        RuleFor(x => x.FirstName).RequiredText(100);
        RuleFor(x => x.LastName).RequiredText(100);
        RuleFor(x => x.CompanyName).OptionalText(200);
        RuleFor(x => x.Email).OptionalEmail();
        RuleFor(x => x.Phone).OptionalText(30);
        RuleFor(x => x.Source).OptionalText(100);
        RuleFor(x => x).Must(x =>
                !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Lead için e-posta veya telefon bilgilerinden biri gereklidir.");
    }
}

public sealed class UpdateLeadDtoValidator : AbstractValidator<UpdateLeadDto>
{
    public UpdateLeadDtoValidator()
    {
        RuleFor(x => x.FirstName).RequiredText(100);
        RuleFor(x => x.LastName).RequiredText(100);
        RuleFor(x => x.CompanyName).OptionalText(200);
        RuleFor(x => x.Email).OptionalEmail();
        RuleFor(x => x.Phone).OptionalText(30);
        RuleFor(x => x.Source).OptionalText(100);
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
    }
}

public sealed class CreateLeadCommandValidator : AbstractValidator<CreateLeadCommand>
{
    public CreateLeadCommandValidator(IValidator<CreateLeadDto> validator) =>
        RuleFor(x => x.Lead).NotNull().SetValidator(validator);
}

public sealed class UpdateLeadCommandValidator : AbstractValidator<UpdateLeadCommand>
{
    public UpdateLeadCommandValidator(IValidator<UpdateLeadDto> validator)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Lead).NotNull().SetValidator(validator);
    }
}

public sealed class GetLeadsQueryValidator : AbstractValidator<GetLeadsQuery>
{
    public GetLeadsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class ConvertLeadCommandValidator : AbstractValidator<ConvertLeadCommand>
{
    public ConvertLeadCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ContactId).NotEmpty();
    }
}
