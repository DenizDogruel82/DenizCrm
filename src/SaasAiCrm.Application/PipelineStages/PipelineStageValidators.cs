using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.PipelineStages;

public sealed class CreatePipelineStageDtoValidator : AbstractValidator<CreatePipelineStageDto>
{
    public CreatePipelineStageDtoValidator()
    {
        RuleFor(x => x.Name).RequiredText(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.WinProbability).InclusiveBetween(0, 100);
        RuleFor(x => x.Color).Matches("^#[0-9A-Fa-f]{6}$");
    }
}

public sealed class UpdatePipelineStageDtoValidator : AbstractValidator<UpdatePipelineStageDto>
{
    public UpdatePipelineStageDtoValidator()
    {
        RuleFor(x => x.Name).RequiredText(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.WinProbability).InclusiveBetween(0, 100);
        RuleFor(x => x.Color).Matches("^#[0-9A-Fa-f]{6}$");
    }
}
