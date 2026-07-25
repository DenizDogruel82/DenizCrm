using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.AiInsights;

public sealed class CreateAiInsightDtoValidator : AbstractValidator<CreateAiInsightDto>
{
    public CreateAiInsightDtoValidator()
    {
        RuleFor(x => x.Title).RequiredText(200);
        RuleFor(x => x.Content).RequiredText(5000);
        RuleFor(x => x.Model).RequiredText(100);
        RuleFor(x => x.Score).InclusiveBetween(0, 100).When(x => x.Score.HasValue);
        RuleFor(x => x.Confidence).InclusiveBetween(0, 1).When(x => x.Confidence.HasValue);
        RuleFor(x => x).Must(x => x.CustomerId.HasValue || x.LeadId.HasValue ||
                                  x.OpportunityId.HasValue)
            .WithMessage("AI içgörüsü en az bir CRM kaydıyla ilişkilendirilmelidir.");
    }
}
