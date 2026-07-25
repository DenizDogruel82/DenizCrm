using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Tenants;

public sealed class CreateTenantDtoValidator : AbstractValidator<CreateTenantDto>
{
    public CreateTenantDtoValidator()
    {
        RuleFor(x => x.Name).RequiredText(200);
        RuleFor(x => x.Slug)
            .RequiredText(100)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$");
        RuleFor(x => x.LogoUrl).OptionalText(500);
        RuleFor(x => x.TimeZone).RequiredText(100);
        RuleFor(x => x.Currency).RequiredText(3).Length(3);
    }
}

public sealed class UpdateTenantDtoValidator : AbstractValidator<UpdateTenantDto>
{
    public UpdateTenantDtoValidator()
    {
        RuleFor(x => x.Name).RequiredText(200);
        RuleFor(x => x.LogoUrl).OptionalText(500);
        RuleFor(x => x.TimeZone).RequiredText(100);
        RuleFor(x => x.Currency).RequiredText(3).Length(3);
    }
}
