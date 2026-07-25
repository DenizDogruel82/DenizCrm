using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Opportunities;

public sealed class CreateOpportunityDtoValidator : AbstractValidator<CreateOpportunityDto>
{
    public CreateOpportunityDtoValidator()
    {
        RuleFor(x => x.Title).RequiredText(200);
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.PipelineStageId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).RequiredText(3).Length(3);
        RuleFor(x => x.Probability).InclusiveBetween(0, 100);
    }
}

public sealed class UpdateOpportunityDtoValidator : AbstractValidator<UpdateOpportunityDto>
{
    public UpdateOpportunityDtoValidator()
    {
        RuleFor(x => x.Title).RequiredText(200);
        RuleFor(x => x.PipelineStageId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).RequiredText(3).Length(3);
        RuleFor(x => x.Probability).InclusiveBetween(0, 100);
        RuleFor(x => x.LostReason).OptionalText(500);
    }
}

public sealed class CreateOpportunityCommandValidator
    : AbstractValidator<CreateOpportunityCommand>
{
    public CreateOpportunityCommandValidator(IValidator<CreateOpportunityDto> validator) =>
        RuleFor(x => x.Opportunity).NotNull().SetValidator(validator);
}

public sealed class UpdateOpportunityCommandValidator
    : AbstractValidator<UpdateOpportunityCommand>
{
    public UpdateOpportunityCommandValidator(IValidator<UpdateOpportunityDto> validator)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Opportunity).NotNull().SetValidator(validator);
    }
}

public sealed class GetOpportunitiesQueryValidator : AbstractValidator<GetOpportunitiesQuery>
{
    public GetOpportunitiesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
