using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Customers;

public sealed class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(x => x.Name).RequiredText(200);
        RuleFor(x => x.Email).OptionalEmail();
        RuleFor(x => x.Phone).OptionalText(30);
        RuleFor(x => x.Website).OptionalText(300);
        RuleFor(x => x.Industry).OptionalText(100);
        RuleFor(x => x.TaxNumber).OptionalText(50);
        RuleFor(x => x.Address).OptionalText(500);
        RuleFor(x => x.City).OptionalText(100);
        RuleFor(x => x.Country).OptionalText(100);
    }
}

public sealed class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(x => x.Name).RequiredText(200);
        RuleFor(x => x.Email).OptionalEmail();
        RuleFor(x => x.Phone).OptionalText(30);
        RuleFor(x => x.Website).OptionalText(300);
        RuleFor(x => x.Industry).OptionalText(100);
        RuleFor(x => x.TaxNumber).OptionalText(50);
        RuleFor(x => x.Address).OptionalText(500);
        RuleFor(x => x.City).OptionalText(100);
        RuleFor(x => x.Country).OptionalText(100);
    }
}

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator(IValidator<CreateCustomerDto> validator) =>
        RuleFor(x => x.Customer).NotNull().SetValidator(validator);
}

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator(IValidator<UpdateCustomerDto> validator)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Customer).NotNull().SetValidator(validator);
    }
}

public sealed class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(200);
    }
}
