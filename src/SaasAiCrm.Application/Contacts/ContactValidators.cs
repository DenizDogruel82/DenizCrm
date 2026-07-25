using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Contacts;

public sealed class CreateContactDtoValidator : AbstractValidator<CreateContactDto>
{
    public CreateContactDtoValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.FirstName).RequiredText(100);
        RuleFor(x => x.LastName).RequiredText(100);
        RuleFor(x => x.JobTitle).OptionalText(150);
        RuleFor(x => x.Email).OptionalEmail();
        RuleFor(x => x.Phone).OptionalText(30);
    }
}

public sealed class UpdateContactDtoValidator : AbstractValidator<UpdateContactDto>
{
    public UpdateContactDtoValidator()
    {
        RuleFor(x => x.FirstName).RequiredText(100);
        RuleFor(x => x.LastName).RequiredText(100);
        RuleFor(x => x.JobTitle).OptionalText(150);
        RuleFor(x => x.Email).OptionalEmail();
        RuleFor(x => x.Phone).OptionalText(30);
    }
}

public sealed class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator(IValidator<CreateContactDto> validator) =>
        RuleFor(x => x.Contact).NotNull().SetValidator(validator);
}

public sealed class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator(IValidator<UpdateContactDto> validator)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Contact).NotNull().SetValidator(validator);
    }
}
